#pragma once

#define GLEW_STATIC
#include <GL/glew.h>
#include "CompilerShader.h"
#include <iostream>

class LinkerShader
{
	GLuint handle = 0;
	bool success = false;
	static constexpr unsigned int LOG_SIZE = 512;
public:
	LinkerShader(std::initializer_list<CompilerShader*> shaders);
	~LinkerShader();
	bool Success() { return success; }
	GLuint Handle() { return handle; }
};

