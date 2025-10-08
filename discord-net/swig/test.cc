#include <iostream>
#include <random>

#include "src/Hello.h"

int main() {
  auto test_class = stl_mt19937(std::random_device()());
  for (auto i = 0; i < 5; ++i) {
    std::cout << test_class.HelloZig("Michael");
  }
}
