// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <time.h>
#include "mkl.h"
#include <iostream>
#include "Header.h"

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

bool CalcMKL(const int nx, double* x, const int ny, double* y, double* lim, const int ns, double* coeff,
	double* result, int& error, double* int_left, double* int_right, double* int_res) {
	try {
		int status;
		DFTaskPtr task;

		status = dfdNewTask1D(&task, nx, x, DF_SORTED_DATA, ny, y, DF_MATRIX_STORAGE_ROWS);      //task creation
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

		status = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_1ST_LEFT_DER | DF_BC_1ST_RIGHT_DER, lim, DF_NO_IC, NULL, coeff, DF_NO_HINT); // task settings
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

		status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD); //
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

		status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, nx, x, DF_SORTED_DATA, 1, new int[1]{ 1 }, NULL,
			result, DF_MATRIX_STORAGE_ROWS, NULL);
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

		status = dfdIntegrate1D(task, DF_METHOD_PP, 1, int_left, DF_SORTED_DATA, int_right, DF_SORTED_DATA, NULL, NULL,
			int_res, DF_MATRIX_STORAGE_ROWS);

		status = dfDeleteTask(&task);
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

		return true;
	}
	catch (...) {
		throw "Error in MKL part\n";
		return false;
	}
}

extern "C"  _declspec(dllexport)
void Hello() {
	std::cout << "hello!\n";
}