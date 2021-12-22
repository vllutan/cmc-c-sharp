#include <time.h>
#include "mkl.h"
#include "mkl_vml_functions.h"
#include <iostream>
#include "Header.h"


//extern "C"  _declspec(dllexport)
bool CalcMKL(const int nx, double* x, const int ny, double* y, const int ns, double* coeff, double* result, int& error) {
	try {
		int status;
		DFTaskPtr task;

		status = dfdNewTask1D(&task, nx, x, DF_UNIFORM_PARTITION, ny, y, DF_MATRIX_STORAGE_ROWS);      //task creation
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

		status = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_FREE_END, NULL, DF_NO_IC, NULL, coeff, DF_NO_HINT);
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

		status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD); // _PP
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

		status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, ns, x, DF_UNIFORM_PARTITION, 1, new int[1]{ 1 }, NULL,
			result, DF_MATRIX_STORAGE_ROWS, NULL);
		if (status != DF_STATUS_OK) {
			error = status;
			return false;
		}

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

//extern "C"  _declspec(dllexport)
void Hello() {
	std::cout << "hello!\n";
}