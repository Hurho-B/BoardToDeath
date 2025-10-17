Shader "Custom/URP_Triplanar_Tessellation"
{
    Properties
    {
        _BaseMap ("Base Color", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _MetallicMap ("Metallic Map", 2D) = "white" {}
        _AOMap ("Ambient Occlusion", 2D) = "white" {}
        _HeightMap ("Height Map", 2D) = "gray" {}
        
        _Tiling ("Tiling", Float) = 1.0
        _BlendSharpness ("Blend Sharpness", Range(1, 16)) = 4.0
        
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _SmoothnessStrength ("Smoothness Strength", Range(0, 2)) = 1.0
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        
        _TessellationFactor ("Tessellation Amount", Range(1, 64)) = 16.0
        _DisplacementStrength ("Displacement Strength", Range(0, 2)) = 0.1
        _TessellationEdgeLength ("Tessellation Edge Length", Range(5, 100)) = 50.0
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma target 4.6
            #pragma require tessellation
            
            #pragma vertex TessellationVertexProgram
            #pragma hull HullProgram
            #pragma domain DomainProgram
            #pragma fragment FragmentProgram
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicMap);
            SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_AOMap);
            SAMPLER(sampler_AOMap);
            TEXTURE2D(_HeightMap);
            SAMPLER(sampler_HeightMap);
            
            CBUFFER_START(UnityPerMaterial)
                float _Tiling;
                float _BlendSharpness;
                float _Smoothness;
                float _SmoothnessStrength;
                float _Metallic;
                float _TessellationFactor;
                float _DisplacementStrength;
                float _TessellationEdgeLength;
            CBUFFER_END
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };
            
            struct TessellationControlPoint
            {
                float4 positionOS : INTERNALTESSPOS;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };
            
            struct TessellationFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };
            
            // Triplanar sampling
            float3 TriplanarBlend(float3 normal)
            {
                float3 blend = abs(normal);
                blend = pow(blend, _BlendSharpness);
                blend /= (blend.x + blend.y + blend.z);
                return blend;
            }
            
            float4 TriplanarSample(TEXTURE2D_PARAM(tex, samp), float3 posWS, float3 blend)
            {
                float2 uvX = posWS.zy * _Tiling;
                float2 uvY = posWS.xz * _Tiling;
                float2 uvZ = posWS.xy * _Tiling;
                
                float4 colX = SAMPLE_TEXTURE2D(tex, samp, uvX);
                float4 colY = SAMPLE_TEXTURE2D(tex, samp, uvY);
                float4 colZ = SAMPLE_TEXTURE2D(tex, samp, uvZ);
                
                return colX * blend.x + colY * blend.y + colZ * blend.z;
            }
            
            float3 TriplanarNormal(float3 posWS, float3 blend, float3 normalWS)
            {
                float2 uvX = posWS.zy * _Tiling;
                float2 uvY = posWS.xz * _Tiling;
                float2 uvZ = posWS.xy * _Tiling;
                
                // Sample and unpack normals for each axis
                float3 tnormalX = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvX));
                float3 tnormalY = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvY));
                float3 tnormalZ = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvZ));
                
                // Swizzle normals to match world space orientations
                // X axis projection (zy plane)
                float3 normalX = float3(tnormalX.y, tnormalX.z, tnormalX.x);
                if (normalWS.x < 0) normalX.x = -normalX.x;
                
                // Y axis projection (xz plane)
                float3 normalY = float3(tnormalY.x, tnormalY.z, tnormalY.y);
                if (normalWS.y < 0) normalY.y = -normalY.y;
                
                // Z axis projection (xy plane)
                float3 normalZ = float3(tnormalZ.x, tnormalZ.y, tnormalZ.z);
                if (normalWS.z < 0) normalZ.z = -normalZ.z;
                
                // Blend the normals
                float3 blendedNormal = normalize(
                    normalX * blend.x +
                    normalY * blend.y +
                    normalZ * blend.z
                );
                
                return blendedNormal;
            }
            
            TessellationControlPoint TessellationVertexProgram(Attributes v)
            {
                TessellationControlPoint o;
                o.positionOS = v.positionOS;
                o.normalOS = v.normalOS;
                o.tangentOS = v.tangentOS;
                return o;
            }
            
            float CalcDistanceTessFactor(float4 vertex, float minDist, float maxDist, float tess)
            {
                float3 worldPosition = TransformObjectToWorld(vertex.xyz);
                float dist = distance(worldPosition, _WorldSpaceCameraPos);
                float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0);
                return f * tess;
            }
            
            TessellationFactors PatchConstantFunction(InputPatch<TessellationControlPoint, 3> patch)
            {
                TessellationFactors f;
                
                float minDist = 10.0;
                float maxDist = 100.0;
                
                f.edge[0] = CalcDistanceTessFactor(patch[1].positionOS, minDist, maxDist, _TessellationFactor);
                f.edge[1] = CalcDistanceTessFactor(patch[2].positionOS, minDist, maxDist, _TessellationFactor);
                f.edge[2] = CalcDistanceTessFactor(patch[0].positionOS, minDist, maxDist, _TessellationFactor);
                f.inside = (f.edge[0] + f.edge[1] + f.edge[2]) / 3.0;
                
                return f;
            }
            
            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [partitioning("integer")]
            [patchconstantfunc("PatchConstantFunction")]
            TessellationControlPoint HullProgram(
                InputPatch<TessellationControlPoint, 3> patch,
                uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }
            
            [domain("tri")]
            Varyings DomainProgram(
                TessellationFactors factors,
                OutputPatch<TessellationControlPoint, 3> patch,
                float3 barycentricCoordinates : SV_DomainLocation)
            {
                Attributes v;
                
                #define DOMAIN_INTERPOLATE(fieldName) v.fieldName = \
                    patch[0].fieldName * barycentricCoordinates.x + \
                    patch[1].fieldName * barycentricCoordinates.y + \
                    patch[2].fieldName * barycentricCoordinates.z;
                
                DOMAIN_INTERPOLATE(positionOS)
                DOMAIN_INTERPOLATE(normalOS)
                DOMAIN_INTERPOLATE(tangentOS)
                
                // Sample height for displacement
                float3 posWS = TransformObjectToWorld(v.positionOS.xyz);
                float3 normalWS = normalize(TransformObjectToWorldNormal(v.normalOS));
                
                // Calculate blend weights consistently
                float3 blend = abs(normalWS);
                blend = pow(blend, _BlendSharpness);
                blend /= (blend.x + blend.y + blend.z);
                
                // Manual triplanar sampling with LOD for domain shader
                float2 uvX = posWS.zy * _Tiling;
                float2 uvY = posWS.xz * _Tiling;
                float2 uvZ = posWS.xy * _Tiling;
                
                float heightX = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, uvX, 0).r;
                float heightY = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, uvY, 0).r;
                float heightZ = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, uvZ, 0).r;
                float height = heightX * blend.x + heightY * blend.y + heightZ * blend.z;
                
                // Apply displacement
                v.positionOS.xyz += v.normalOS * (height - 0.5) * _DisplacementStrength;
                
                Varyings o;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS = TransformWorldToHClip(o.positionWS);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                
                float sign = v.tangentOS.w * GetOddNegativeScale();
                o.tangentWS = float4(TransformObjectToWorldDir(v.tangentOS.xyz), sign);
                
                // Calculate shadow coord
                #if SHADOWS_SCREEN
                    o.shadowCoord = ComputeScreenPos(o.positionCS);
                #else
                    o.shadowCoord = TransformWorldToShadowCoord(o.positionWS);
                #endif
                
                return o;
            }
            
            half4 FragmentProgram(Varyings i) : SV_Target
            {
                float3 normalWS = normalize(i.normalWS);
                float3 blend = TriplanarBlend(normalWS);
                
                // Sample textures
                float4 baseColor = TriplanarSample(TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap), i.positionWS, blend);
                float metallic = TriplanarSample(TEXTURE2D_ARGS(_MetallicMap, sampler_MetallicMap), i.positionWS, blend).r * _Metallic;
                float ao = TriplanarSample(TEXTURE2D_ARGS(_AOMap, sampler_AOMap), i.positionWS, blend).r;
                float smoothness = saturate(_Smoothness * _SmoothnessStrength);
                
                // Apply triplanar normal mapping
                normalWS = TriplanarNormal(i.positionWS, blend, normalWS);
                normalWS = normalize(normalWS);
                
                // Get main light
                Light mainLight = GetMainLight(i.shadowCoord);
                
                // View direction
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(i.positionWS));
                
                // Calculate lighting
                float3 halfDir = normalize(mainLight.direction + viewDirWS);
                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float NdotH = saturate(dot(normalWS, halfDir));
                float NdotV = saturate(dot(normalWS, viewDirWS));
                float LdotH = saturate(dot(mainLight.direction, halfDir));
                
                // PBR calculations
                float perceptualRoughness = 1.0 - smoothness;
                float roughness = perceptualRoughness * perceptualRoughness;
                float roughness2 = roughness * roughness;
                float roughness2MinusOne = roughness2 - 1.0;
                
                // GGX Normal Distribution
                float d = NdotH * NdotH * roughness2MinusOne + 1.00001;
                float D = roughness2 / (3.14159265359 * d * d);
                
                // Geometry term (Smith)
                float k = roughness * 0.5;
                float gV = NdotV / (NdotV * (1.0 - k) + k);
                float gL = NdotL / (NdotL * (1.0 - k) + k);
                float G = gV * gL;
                
                // Fresnel (Schlick approximation)
                float3 F0 = lerp(float3(0.04, 0.04, 0.04), baseColor.rgb, metallic);
                float fresnel = pow(1.0 - LdotH, 5.0);
                float3 F = F0 + (1.0 - F0) * fresnel;
                
                // Specular BRDF
                float3 specular = (D * G * F) / max(4.0 * NdotL * NdotV, 0.001);
                specular *= mainLight.color * NdotL;
                
                // Diffuse
                float3 kD = (1.0 - F) * (1.0 - metallic);
                float3 diffuse = kD * baseColor.rgb * mainLight.color * NdotL;
                
                // Direct lighting
                float3 directLighting = (diffuse + specular) * mainLight.shadowAttenuation;
                
                // Indirect lighting (ambient)
                float3 ambient = SampleSH(normalWS) * baseColor.rgb * ao;
                
                // For metals, add environment reflection approximation
                float3 reflectVector = reflect(-viewDirWS, normalWS);
                float3 indirectSpecular = SampleSH(reflectVector) * F0 * smoothness * metallic;
                
                // Combine
                float3 finalColor = directLighting + ambient + indirectSpecular;
                
                return half4(finalColor, 1.0);
            }
            
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            HLSLPROGRAM
            #pragma target 4.6
            #pragma require tessellation
            
            #pragma vertex TessVert
            #pragma hull Hull
            #pragma domain Domain
            #pragma fragment ShadowFrag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            TEXTURE2D(_HeightMap);
            SAMPLER(sampler_HeightMap);
            
            float _Tiling;
            float _BlendSharpness;
            float _TessellationFactor;
            float _DisplacementStrength;
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };
            
            struct TessVert2Tess
            {
                float4 positionOS : INTERNALTESSPOS;
                float3 normalOS : NORMAL;
            };
            
            struct TessFactors
            {
                float edge[3] : SV_TessFactor;
                float inside : SV_InsideTessFactor;
            };
            
            TessVert2Tess TessVert(Attributes v)
            {
                TessVert2Tess o;
                o.positionOS = v.positionOS;
                o.normalOS = v.normalOS;
                return o;
            }
            
            TessFactors PatchFunc(InputPatch<TessVert2Tess, 3> patch)
            {
                TessFactors f;
                f.edge[0] = _TessellationFactor;
                f.edge[1] = _TessellationFactor;
                f.edge[2] = _TessellationFactor;
                f.inside = _TessellationFactor;
                return f;
            }
            
            [domain("tri")]
            [outputcontrolpoints(3)]
            [outputtopology("triangle_cw")]
            [partitioning("integer")]
            [patchconstantfunc("PatchFunc")]
            TessVert2Tess Hull(InputPatch<TessVert2Tess, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }
            
            [domain("tri")]
            float4 Domain(TessFactors factors, OutputPatch<TessVert2Tess, 3> patch, float3 bary : SV_DomainLocation) : SV_POSITION
            {
                Attributes v;
                v.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
                v.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
                
                float3 posWS = TransformObjectToWorld(v.positionOS.xyz);
                float3 normalWS = TransformObjectToWorld(v.normalOS);
                float3 blend = abs(normalWS);
                blend = pow(blend, _BlendSharpness);
                blend /= (blend.x + blend.y + blend.z);
                
                float2 uvX = posWS.zy * _Tiling;
                float2 uvY = posWS.xz * _Tiling;
                float2 uvZ = posWS.xy * _Tiling;
                
                float heightX = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, uvX, 0).r;
                float heightY = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, uvY, 0).r;
                float heightZ = SAMPLE_TEXTURE2D_LOD(_HeightMap, sampler_HeightMap, uvZ, 0).r;
                float height = heightX * blend.x + heightY * blend.y + heightZ * blend.z;
                
                v.positionOS.xyz += v.normalOS * (height - 0.5) * _DisplacementStrength;
                
                return TransformWorldToHClip(ApplyShadowBias(TransformObjectToWorld(v.positionOS.xyz), normalWS, _MainLightPosition.xyz));
            }
            
            half4 ShadowFrag() : SV_Target
            {
                return 0;
            }
            
            ENDHLSL
        }
    }
}