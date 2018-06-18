#include <iostream>
#include <fstream>

const char *readShaderSource(const char* path)
{
	std::ifstream ifs(path);
	if (!ifs.is_open())
	{
		std::cerr << "Not such file exists " << path << std::endl;
		return nullptr;
	}

	std::string strSource((std::istreambuf_iterator<char>(ifs)), std::istreambuf_iterator<char>());
	const char *source = strSource.c_str();
	size_t length = strSource.length();
	char *dest = new char[length + 1];
	strcpy_s(dest, length + 1, source);
	return dest;
}