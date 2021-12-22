#pragma once


extern "C" __declspec(dllexport) bool CalcMKL(const int nx, double* x, const int ny, double* y, int ns, double* coeff, double* result, int& error);
extern "C" __declspec(dllexport) void Hello();
