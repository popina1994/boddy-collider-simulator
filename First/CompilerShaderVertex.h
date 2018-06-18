#pragma once
#include "CompilerShader.h"
class CompilerShaderVertex :
	public CompilerShader
{
public:
	CompilerShaderVertex(const char* srcPath): 
		CompilerShader(GL_VERTEX_SHADER, srcPath){}
	~CompilerShaderVertex() {}
};

