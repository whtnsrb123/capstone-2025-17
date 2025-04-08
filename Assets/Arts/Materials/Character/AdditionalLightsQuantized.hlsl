void AdditionalLightsQuantized_float(float3 WorldPosition, float3 WorldNormal, out float3 Diffuse)
{
    float3 AdditionalLightColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
        uint pixelLightCount = GetAdditionalLightsCount();

        LIGHT_LOOP_BEGIN(pixelLightCount)
            Light light = GetAdditionalLight(lightIndex, WorldPosition);
            float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
            
            // Lambert 연산 수행
            float NdotL = saturate(dot(WorldNormal, light.direction));

            //// 양자화 처리 (4단계)
            float quantizeSteps = 4.0;
            float quantizedDiffuse = ceil(NdotL * quantizeSteps) / quantizeSteps;

            AdditionalLightColor += attenuatedLightColor * quantizedDiffuse;
        LIGHT_LOOP_END
    #endif

    Diffuse = AdditionalLightColor;
}