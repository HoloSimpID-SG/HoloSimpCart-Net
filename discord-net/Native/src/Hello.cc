#include "Hello.h"

#include <cstdint>
#include <cstdlib>
#include <format>
#include <random>
#include <string>

extern "C" {
const char* zig_hello(const char* name, int number);
int32_t zig_num(int32_t number);
}

stl_mt19937::stl_mt19937(uint32_t seed) : mt_(std::random_device()()) {}

int stl_mt19937::GetRandom() { return std::uniform_int_distribution<int>(0, INT32_MAX - 1)(mt_); }

std::string stl_mt19937::HelloZig(std::string name) {
  int rand            = GetRandom();
  const char* zig_str = zig_hello(name.c_str(), GetRandom());
  std::string res     = std::string(zig_str);
  free((void*)zig_str);
  return name;
}

std::string stl_mt19937::ZigNum() {
  int rand = GetRandom();
  return std::format("{} --> {}", rand, zig_num(rand));
}
