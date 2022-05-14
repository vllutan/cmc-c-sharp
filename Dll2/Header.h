#pragma once


extern "C" __declspec(dllexport) int CalcMKL(int nx, double* x, int ny, double* y, double* lim, int ns, double* site, double* coeff,
	double* result, int& error, double* int_left, double* int_right, double* int_res);
