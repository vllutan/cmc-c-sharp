#include <time.h>
#include "mkl.h"
#include "mkl_vml_functions.h"
#include <iostream>
#include "Header.h"


//extern "C"  _declspec(dllexport)
int CalcMKL(int nx, double* x, int ny, double* y, double* lim, int ns, double* site, double* coeff, 
	double* result, int& error, double* int_left, double* int_right, double* int_res) {
	try {
		int status = 0;
		DFTaskPtr task;

		status = dfdNewTask1D(&task, nx, x, DF_NON_UNIFORM_PARTITION, ny, y, DF_MATRIX_STORAGE_ROWS);      // DF_SORTED_DATA DF_NON_UNIFORM_PARTITION
		if (status != DF_STATUS_OK) {
			error = status;
			return 1;
		}

		status = dfdEditPPSpline1D(task, DF_PP_CUBIC, DF_PP_NATURAL, DF_BC_1ST_LEFT_DER | DF_BC_1ST_RIGHT_DER, lim, 
			DF_NO_IC, NULL, coeff, DF_NO_HINT);
		if (status != DF_STATUS_OK) {
			error = status;
			std::cout << status;
			return 2;
		}

		status = dfdConstruct1D(task, DF_PP_SPLINE, DF_METHOD_STD); 
		if (status != DF_STATUS_OK) {
			error = status;
			return 3;
		}

		status = dfdInterpolate1D(task, DF_INTERP, DF_METHOD_PP, ns, site, DF_UNIFORM_PARTITION, 1, new int[1]{ 1 }, NULL,
			result, DF_MATRIX_STORAGE_ROWS, NULL);
		if (status != DF_STATUS_OK) {
			error = status;
			return 4;
		}

		status = dfdIntegrate1D(task, DF_METHOD_PP, 1, int_left, DF_SORTED_DATA, int_right, DF_SORTED_DATA, NULL, NULL, 
			int_res, DF_MATRIX_STORAGE_ROWS);
		if (status != DF_STATUS_OK) {
			error = status;
			return 5;
		}

		status = dfDeleteTask(&task);
		if (status != DF_STATUS_OK) {
			error = status;
			return 6;
		}

		return 0;
	}
	catch (...) {
		throw "Error in MKL part\n";
		return -1;
	}
}