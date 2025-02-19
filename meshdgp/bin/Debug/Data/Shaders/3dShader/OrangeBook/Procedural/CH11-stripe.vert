//
// Vertex shader for drawing procedural stripes
//
// Author: OGLSL implementation by Ian Nurse
//
// Copyright (C) 2002-2006  LightWork Design Ltd.
//          www.lightworkdesign.com
//
// See LightworkDesign-License.txt for license information
//

uniform vec3  LightPosition;
uniform vec3  LightColor;
uniform vec3  EyePosition;
uniform vec3  Specular;
uniform vec3  Ambient;
uniform float Kd;

varying vec3  DiffuseColor;
varying vec3  SpecularColor;

void main()
{
    vec3 ecPosition = vec3(gl_ModelViewMatrix * gl_Vertex);
    vec3 tnorm      = normalize(gl_NormalMatrix * gl_Normal);
    vec3 lightVec   = normalize(LightPosition - ecPosition);
    vec3 viewVec    = normalize(EyePosition - ecPosition);
    vec3 Hvec       = normalize(viewVec + lightVec);

    float spec = clamp(dot(Hvec, tnorm), 0.0, 1.0);
    spec = pow(spec, 16.0);

    DiffuseColor    = LightColor * vec3(Kd * dot(lightVec, tnorm));
    DiffuseColor    = clamp(Ambient + DiffuseColor, 0.0, 1.0);
    SpecularColor   = clamp((LightColor * Specular * spec), 0.0, 1.0);

	if(gl_MultiTexCoord0.s==0&&gl_MultiTexCoord0.t==0)
	     gl_TexCoord[0] = gl_Vertex;
	else
         gl_TexCoord[0]  = gl_MultiTexCoord0;
    gl_Position     = ftransform();
}