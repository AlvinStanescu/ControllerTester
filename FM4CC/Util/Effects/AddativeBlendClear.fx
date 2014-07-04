// Creative Commons Attribution 3.0 United States License
// http://creativecommons.org/licenses/by/3.0/us/
//
// Copyright (c) 2010 Nick Darnell
// http://www.nickdarnell.com

/// <description>An effect that controls brightness and contrast.</description>
/// <profile>ps_2_0</profile>

//-----------------------------------------------------------------------------
// Constants
//-----------------------------------------------------------------------------

/// <summary>The brightness offset.</summary>
/// <defaultValue>1,1,1,1</defaultValue>
float4 ClearColor : register(c0);

//-----------------------------------------------------------------------------
// Samplers
//-----------------------------------------------------------------------------

/// <summary>The implicit input sampler passed into the pixel shader by WPF.</summary>
sampler2D Input : register(s0);

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 pixelColor = tex2D(Input, uv);
    return ClearColor * ClearColor.a + pixelColor;
}