#pragma once

#define GLEW_STATIC
#include <GL/glew.h>

class CompilerShader
{
	GLuint handle = 0;
	bool success = false;
	static constexpr unsigned int LOG_SIZE = 512;
public:
	CompilerShader(GLenum _type, const char* _sourcePath);
	virtual ~CompilerShader() { glDeleteShader(handle); }

	bool Success() { return success;  }
	GLuint Handle() { return handle;  }
	
};