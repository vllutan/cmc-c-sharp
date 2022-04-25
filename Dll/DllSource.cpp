#include <time.h>
#include "mkl.h"
#include "mkl_vml_functions.h"
#include <iostream>
#include "DllHeader.h"
#include <string>


//extern "C"  _declspec(dllexport)
bool Sin(int len, double* inp_vec, double* out_vec,int mode) {
	try {
		MKL_INT64 mode_mkl;
		if (mode == 1)  mode_mkl = VML_HA | VML_FTZDAZ_OFF | VML_ERRMODE_DEFAULT;
		else if (mode == 2) mode_mkl = VML_LA | VML_FTZDAZ_ON | VML_ERRMODE_DEFAULT;
		else if (mode == 3) mode_mkl = VML_EP | VML_FTZDAZ_ON | VML_ERRMODE_DEFAULT;
		else return false;

		vmdSin(len, inp_vec, out_vec, mode_mkl);
		return true;
	}
	catch (...) {
		throw "Error in MKL Sin part\n";
		return false;
	}
}

bool Cos(int len, double* inp_vec, double* out_vec, int mode){
	try {
		MKL_INT64 mode_mkl;
		if (mode == 1) mode_mkl = VML_HA;
		else if (mode == 2) mode_mkl = VML_LA;
		else if (mode == 3) mode_mkl = VML_EP;
		else return false;

		vmdCos(len, inp_vec, out_vec, mode_mkl);
		return true;
	}
	catch (...) {
		throw "Error in MKL Sin part\n";
		return false;
	}
}

bool SinCos(int len, double* inp_vec, double* out_vec1, double* out_vec2, int mode) {
	try {
		MKL_INT64 mode_mkl;
		if (mode == 1) mode_mkl = VML_HA;
		else if (mode == 2) mode_mkl = VML_LA;
		else if (mode == 3) mode_mkl = VML_EP;
		else return false;

		vmdSinCos(len, inp_vec, out_vec1, out_vec2, mode_mkl);
		return true;
	}
	catch (...) {
		throw "Error in MKL Sin part\n";
		return false;
	}
}
