#include "CompilerShader.h"
#include "utils.h"
#include <iostream>

CompilerShader::CompilerShader(GLenum type, const char* sourcePath)
{
	success = true;
	handle = glCreateShader(type);
	const GLchar *shaderSource = readShaderSource(sourcePath);
	glShaderSource(handle, 1, &shaderSource, NULL);
	glCompileShader(handle);
	delete shaderSource;

	GLint glSuccess;
	GLchar infoLog[LOG_SIZE];

	glGetShaderiv(handle, GL_COMPILE_STATUS, &glSuccess);
	if (!glSuccess)
	{
		glGetShaderInfoLog(handle, LOG_SIZE, NULL, infoLog);
		std::cerr << "ERROR::SHADER::VERTEX::COMPILATION_FAILED\n" << infoLog << std::endl;
		success = false;
	}
}