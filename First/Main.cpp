#include <iostream>
#include <fstream>
#include <cstring>

// GLEW
#define GLEW_STATIC
#include <GL/glew.h>

// GLFW
#include <GLFW/glfw3.h>

#include "utils.h"
#include "CompilerShaderVertex.h"
#include "CompilerShaderFragment.h"
#include "LinkerShader.h"
#include "callbacks.h"

// Window dimensions
constexpr GLint SCREEN_HEIGHT = 1040;

// Shaders
const char* VERTEX_SOURCE_PATH = "vertex.glsl";
const char* FRAGMENT_SOURCE_PATH = "fragment.glsl";

// The MAIN function, from here we start the application and run the game loop
int main()
{
	FILE *stream;
	freopen_s(&stream, "log.txt", "w", stderr);
	std::ofstream out("log_w.txt");
	std::cout.rdbuf(out.rdbuf());
	// Init GLFW
	glfwInit();
	atexit(glfwTerminate);

	// Set all the required options for GLFW
	glfwWindowHint(GLFW_CONTEXT_VERSION_MAJOR, 4);
	glfwWindowHint(GLFW_CONTEXT_VERSION_MINOR, 5);
	glfwWindowHint(GLFW_OPENGL_PROFILE, GLFW_OPENGL_CORE_PROFILE);
	glfwWindowHint(GLFW_OPENGL_FORWARD_COMPAT, GL_TRUE);
	glfwWindowHint(GLFW_RESIZABLE, GL_FALSE);

	// Create a GLFWwindow object that we can use for GLFW's functions
	const GLFWvidmode *pScreenSize = glfwGetVideoMode(glfwGetPrimaryMonitor());
	GLFWwindow *pWindow = glfwCreateWindow(pScreenSize->width, SCREEN_HEIGHT, "The Mandelbrot Set", nullptr, nullptr);
	if (nullptr == pWindow)
	{
		std::cerr << "Failed to create GLFW pWindow" << std::endl;
		glfwTerminate();
		return EXIT_FAILURE;
	}

	initCallbacks(pWindow, pScreenSize->width, SCREEN_HEIGHT);
	glfwMakeContextCurrent(pWindow);

	// Set this to true so GLEW knows to use a modern approach to retrieving function pointers and extensions
	glewExperimental = GL_TRUE;
	// Initialize GLEW to setup the OpenGL Function pointers
	if (GLEW_OK != glewInit())
	{
		std::cerr << "Failed to initialize GLEW" << std::endl;
		return EXIT_FAILURE;
	}

	GLint screenWidth;
	GLint screenHeight;
	glfwGetFramebufferSize(pWindow, &screenWidth, &screenHeight);
	glViewport(0, 0, screenWidth, screenHeight);
	GLdouble aScreenSize[2] = { screenWidth, screenHeight};
	CompilerShaderVertex compilerVertexShader(VERTEX_SOURCE_PATH);
	CompilerShaderFragment compilerFragmentShader(FRAGMENT_SOURCE_PATH);
	if (!compilerVertexShader.Success() || !compilerFragmentShader.Success())
	{
		return EXIT_FAILURE;
	}
	
	LinkerShader linkerShader({ &compilerVertexShader, &compilerFragmentShader });
	if (!linkerShader.Success())
	{
		return EXIT_FAILURE;
	}

	// Two triangles needed because of OpenGL...
	GLfloat vertices[] = {
		1.0f,  -1.0f,  0.0f,
		-1.0f,  1.0f,  0.0f,
		-1.0f,  -1.0f,  0.0f,
		-1.0f,  1.0f,  0.0f,
		1.0f,  -1.0f,  0.0f,
		1.0f,  1.0f,  0.0f,
	};
	
	GLfloat colorData[] = {
		1.0f, 0.0f, 0.0f,
		0.0f, 1.0f, 0.0f,
		0.0f, 0.0f, 1.0f
	};


	GLuint VBO, VAO;
	glGenVertexArrays(1, &VAO);
	glGenBuffers(1, &VBO);
	glBindVertexArray(VAO);
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(vertices), vertices, GL_STATIC_DRAW);

	glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, 0, nullptr);
	glEnableVertexAttribArray(0);

	glGenBuffers(1, &VBO);
	glBindBuffer(GL_ARRAY_BUFFER, VBO);
	glBufferData(GL_ARRAY_BUFFER, sizeof(colorData), colorData, GL_STATIC_DRAW);
	glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, 0, nullptr);
	glEnableVertexAttribArray(1);
	
	glBindBuffer(GL_ARRAY_BUFFER, 0); 
	glBindVertexArray(0); 

	while (!glfwWindowShouldClose(pWindow))
	{
		GLint uniformScreenSize;
		GLint uniformMaxIteration;
		GLint uniformZoom;
		GLint uniformLeftTop;

		glfwPollEvents();
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
		glUseProgram(linkerShader.Handle());
		
		// Passing uniform variables to shaders. 
		uniformScreenSize = glGetUniformLocation(linkerShader.Handle(), "screenSize");
		uniformMaxIteration = glGetUniformLocation(linkerShader.Handle(), "maxIteration");
		uniformZoom = glGetUniformLocation(linkerShader.Handle(), "zoom");
		uniformLeftTop = glGetUniformLocation(linkerShader.Handle(), "leftTop");

		glUniform2dv(uniformScreenSize, 1, aScreenSize);
		glUniform1d(uniformMaxIteration, maxIteration);
		glUniform1d(uniformZoom, zoom);
		glUniform2dv(uniformLeftTop, 1, leftTop);
		glBindVertexArray(VAO);
		glDrawArrays(GL_TRIANGLES, 0, 6);
		glBindVertexArray(0);

		// Swap the screen buffers
		glfwSwapBuffers(pWindow);
	}

 	glDeleteVertexArrays(1, &VAO);
	glDeleteBuffers(1, &VBO);

	return EXIT_SUCCESS;
}


