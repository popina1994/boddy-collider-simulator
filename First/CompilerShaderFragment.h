#pragma once
#include "CompilerShader.h"
class CompilerShaderFragment :
	public CompilerShader
{
public:
	CompilerShaderFragment(const char* srcPath) :
		CompilerShader(GL_FRAGMENT_SHADER, srcPath) {}
	~CompilerShaderFragment() {}
};
