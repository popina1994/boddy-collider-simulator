#include "LinkerShader.h"
#include "CompilerShader.h"
#include <iostream>


LinkerShader::LinkerShader(std::initializer_list<CompilerShader*> shaders)
{
	handle = glCreateProgram();
	success = true;
	for (auto shader : shaders)
	{
		glAttachShader(handle, shader->Handle());
	}
	glLinkProgram(handle);

	GLint glSuccess;
	GLchar infoLog[LOG_SIZE];
	glGetProgramiv(handle, GL_LINK_STATUS, &glSuccess);
	if (!glSuccess)
	{
		glGetProgramInfoLog(handle, LOG_SIZE, nullptr, infoLog);
		std::cerr << "ERROR::SHADER::PROGRAM::LINKING_FAILED\n" << infoLog << std::endl;
		success = false;
	}
}


LinkerShader::~LinkerShader()
{
	glDeleteProgram(handle);
}
