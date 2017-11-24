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
/// <summary>Gamma</summary>
/// <minValue>0/minValue>
/// <maxValue>10</maxValue>
/// <defaultValue>1</defaultValue>
float Gamma : register(C2);

float4 main(double2 uv : TEXCOORD) : COLOR 
{ 
	
	float4 color; 
  color= tex2D( input , uv.xy);

  if(color.r>=HighRange) {color.r=1;}
  else if(color.r<=LowRange) {color.r=0;}
  else {color.r=pow((color.r-LowRange)/(HighRange-LowRange),1.0/Gamma);}
  if(color.g>=HighRange) {color.g=1;}
  else if(color.g<=LowRange) {color.g=0;}
  else {color.g=pow((color.g-LowRange)/(HighRange-LowRange),1.0/Gamma);}
  if(color.b>=HighRange) {color.b=1;}
  else if(color.b<=LowRange) {color.b=0;}
  else {color.b=pow((color.b-LowRange)/(HighRange-LowRange),1.0/Gamma);}
//	color.rgb=color.rgb+0.5/(HighRange-LowRange);
	
  return color; 
}