Shader "Custom/DepthMask"
{
	SubShader
	{
		Tags {"Queue" = "Transparent-1" }
		ColorMask 0
		ZWrite On
		Pass {}
	}
}