#pragma once

//GLEW
#define GLEW_STATIC
#include <GL/glew.h>

// GLFW
#include <GLFW/glfw3.h>

extern double zoom;
extern double maxIteration;
extern double leftTop[2];

void initCallbacks(GLFWwindow *pWindow, double screenWidth, double screenHeight);
