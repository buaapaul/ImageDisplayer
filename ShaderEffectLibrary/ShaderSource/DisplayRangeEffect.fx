sampler2D input : register(s0);

// new HLSL shader
// modify the comment parameters to reflect your shader parameters

/// <summary>Low display range</summary>
/// <minValue>0/minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>0</defaultValue>
float LowRange : register(C0);
/// <summary>High display range</summary>
/// <minValue>0/minValue>
/// <maxValue>1</maxValue>
/// <defaultValue>1</defaultValue>
float HighRange : register(C1);

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	
	float4 color; 
	color= tex2D( input , uv.xy);
	
	if( color.r>HighRange ) { color.r=1; }
	else if( color.r>HighRange ) { color.r=0; }
	else{ color.r=(color.r-LowRange)/(HighRange-LowRange); }

	if( color.g>HighRange ) { color.g=1; }
	else if( color.g>HighRange ) { color.g=0; }
	else{ color.g=(color.g-LowRange)/(HighRange-LowRange); }

	if( color.b>HighRange ) { color.b=1; }
	else if( color.b>HighRange ) { color.b=0; }
	else{ color.b=(color.b-LowRange)/(HighRange-LowRange); }

	if( color.a>HighRange ) { color.a=1; }
	else if( color.a>HighRange ) { color.a=0; }
	else{ color.a=(color.a-LowRange)/(HighRange-LowRange); }

  return color; 
}