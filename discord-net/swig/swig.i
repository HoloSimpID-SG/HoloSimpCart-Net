%module swig

%{
#include "src/Hello.h"
%}

%include "stl.i"
%include "stdint.i"
%include "src/Hello.h"
