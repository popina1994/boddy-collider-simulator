#include "callbacks.h"

#include <iostream>
#include <string>

constexpr int MIN_ITERATION = 1;
constexpr int MAX_ITERATION = 360;

double zoom = 1.0;
double leftTop[2] = { 0 };
static double rightBottom[2] = { 0 };
double maxIteration = 100;
static double screenSize[2] = { 0 };
static double xClick = 0.0;
static double yClick = 0.0;
static bool clicked = false;

void scaleAndPrint(double a[], const char* str)
{
	double x = (a[0] * zoom + leftTop[0] - screenSize[0] / 2) * 1 / (screenSize[0] / 2);
	double y = (a[1] * zoom + leftTop[1] - screenSize[1]) / (screenSize[1] / 2);
	std::cout << str << " (" << x << ", " << y << ")" << std::endl;
}

void printScreen(void)
{
	scaleAndPrint(leftTop, "leftTop");
	scaleAndPrint(rightBottom, "rightBottom");
	for (int row = 0; row <= screenSize[0]; row += 50)
	{
		for (int col = 0; col <= screenSize[1]; col += 50)
		{
			double a[2] = { row, col };
			std::string s = (std::to_string(row) + std::to_string(col));
			scaleAndPrint(a, s.c_str());
		}
	}
	std::cout << std::endl;
}

static bool panCoordinate(double xPos, double yPos)
{
	if ((xClick != xPos) || (yClick != yPos))
	{
		leftTop[0] += zoom * (xPos - xClick);
		rightBottom[0] += zoom * (xPos - xClick);
		leftTop[1] += zoom * (yPos - yClick);
		rightBottom[1] += zoom * (yPos - yClick);
		return true;
	}
	return false;
}

static void mouseButtonCallback(GLFWwindow* pWindow, int button, int action, int mods)
{
	double xPos, yPos;
	glfwGetCursorPos(pWindow, &xPos, &yPos);
	yPos = screenSize[1] - yPos;
	if (GLFW_PRESS == action)
	{
		if ((GLFW_MOUSE_BUTTON_LEFT == button) || (GLFW_MOUSE_BUTTON_RIGHT == button))
		{
			clicked = true;
			xClick = xPos;
			yClick = yPos;
		}
	}

	if (GLFW_RELEASE == action)
	{
		clicked = false;
		if (!panCoordinate(xPos, yPos) &&
			((GLFW_MOUSE_BUTTON_LEFT == button) || (GLFW_MOUSE_BUTTON_RIGHT == button)))
		{
			double dim[2];
			double xFixPos = xPos * zoom + leftTop[0];
			double yFixPos = yPos * zoom + leftTop[1];
			double signExp;
			dim[0] = (rightBottom[0] - leftTop[0]) * 0.1;
			dim[1] = (rightBottom[1] - leftTop[1]) * 0.1;

			if (GLFW_MOUSE_BUTTON_LEFT == button)
			{
				zoom = zoom * 0.9;
				signExp = 1;
			}
			else if (GLFW_MOUSE_BUTTON_RIGHT == button)
			{
				zoom = zoom * 1.1;
				signExp = -1;
			}
			leftTop[0] += signExp * dim[0] / 2;
			leftTop[1] += signExp * dim[1] / 2;
			rightBottom[0] -= signExp * dim[0] / 2;
			rightBottom[1] -= signExp * dim[1] / 2;

			double xNewPos = xPos * zoom + leftTop[0];
			double yNewPos = yPos * zoom + leftTop[1];
			double centerXTran = xNewPos - xFixPos;
			double centerYTran = yNewPos - yFixPos;
			leftTop[0] -= centerXTran;
			leftTop[1] -= centerYTran;
			rightBottom[0] -= centerXTran;
			rightBottom[1] -= centerYTran;
		}
	}
}

static void cursorPosCallback(GLFWwindow* pWindow, double xPos, double yPos)
{
	if (clicked)
	{
		yPos = screenSize[1] - yPos;
		//panCoordinate(xPos, yPos);
	}
}

static void scrollCallback(GLFWwindow *pWindows, double xOffset, double yOffset)
{
	double maxItPot = maxIteration + yOffset;
	if (maxItPot < MIN_ITERATION)
	{
		maxItPot = 1;
	}
	if (maxItPot > MAX_ITERATION)
	{
		maxItPot = MAX_ITERATION;
	}
	maxIteration = maxItPot;
}

void initCallbacks(GLFWwindow *pWindow, double screenWidth, double screenHeight)
{

	glfwSetMouseButtonCallback(pWindow, mouseButtonCallback);
	glfwSetCursorPosCallback(pWindow, cursorPosCallback);
	glfwSetScrollCallback(pWindow, scrollCallback);
	screenSize[0] = screenWidth;
	screenSize[1] = screenHeight;
	rightBottom[0] = screenSize[0];
	rightBottom[1] = screenSize[1];
}
