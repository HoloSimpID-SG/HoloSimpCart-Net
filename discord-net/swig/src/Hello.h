#include <cstddef>
#include <ctime>
#include <random>

class stl_mt19937 {
 private:
  std::mt19937 mt_;

 public:
  stl_mt19937(uint32_t seed = time(nullptr));
  int GetRandom();
  std::string HelloZig(std::string name);
  std::string ZigNum();
};
