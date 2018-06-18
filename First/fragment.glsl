#version 450 core

out vec4 outColor;

uniform dvec2 screenSize;
uniform double maxIteration;
uniform dvec2 leftTop;
uniform double zoom;

const double MAND_THRESHOLD = 2;

vec3 hsv2rgb(vec3 c)
{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec4 mapToColor(float t)
{
	vec3 hsv = vec3(t, 1, 1);
	vec3 rgb = hsv2rgb(hsv);
	return vec4(rgb.x, rgb.y, rgb.z, 1);
}

double mandelbroNumItRatio(dvec2 c)
{
	double x, y;
	dvec2 z = dvec2(0, 0);
	int i;
	
	for (i = 0; i < maxIteration; i++) {
		x = z.x * z.x - z.y * z.y + c.x;
		y = 2 * z.y * z.x + c.y;
		if ((x * x + y * y) > MAND_THRESHOLD * MAND_THRESHOLD) break;
		z.x = x;
		z.y = y;
	}
	return i / maxIteration;
}

void main()
{
	double itNum;
	dvec2 c;
	double screenRatio = screenSize.x / screenSize.y;

	// 0 - 800 -> 0-720-> 40-760 -> -360-360->-1 - 1
	c.x = (gl_FragCoord.x * zoom + leftTop[0] - screenSize.x / 2) * screenRatio / (screenSize.x / 2);
	c.y = (gl_FragCoord.y * zoom + leftTop[1] - screenSize.y / 2) / (screenSize.y / 2);
	itNum = mandelbroNumItRatio(c);
	if (itNum >= 1)
	{
		outColor = vec4(0, 0, 0, 1);
	}
	else
	{
		outColor = mapToColor(float(itNum));
	}
};


