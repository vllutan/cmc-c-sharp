#pragma once
#include <string>


//extern "C" __declspec(dllexport) bool CalcMKL(const int nx, double* x, const int ny, double* y, int& error);
extern "C" __declspec(dllexport) bool Sin(int len, double* inp_vec, double* out_vec, int mode);
extern "C" __declspec(dllexport) bool Cos(int len, double* inp_vec, double* out_vec, int mode);
extern "C" __declspec(dllexport) bool SinCos(int len, double* inp_vec, double* out_vec1, double* out_vec2, int mode);
