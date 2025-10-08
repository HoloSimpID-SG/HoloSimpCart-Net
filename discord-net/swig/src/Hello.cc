#include "Hello.h"

#include <cstdint>
#include <cstdlib>
#include <format>
#include <string>

stl_mt19937::stl_mt19937(uint32_t seed) : mt_(seed) {}

int stl_mt19937::GetRandom() { return std::uniform_int_distribution<int>(0, 42)(mt_); }

extern char* zig_hello(const char* name, int32_t number);

std::string stl_mt19937::HelloZig(std::string name) {
  const char* str  = std::format("Hello {}!", name).c_str();
  char* zig_return = zig_hello(str, GetRandom());
  std::string res  = std::string(zig_return);
  free(zig_return);
  return res;
}
