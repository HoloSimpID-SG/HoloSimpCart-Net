const std = @import("std");

pub fn build(b: *std.Build) void {
  const target = b.standardTargetOptions(.{});
  const optimize = b.standardOptimizeOption(.{});
  const lib = b.addSharedLibrary(.{
    .name = "Native",
    .root_source_file = b.path("main.zig"),
    .target = target,
    .optimize = optimize,
  });
  lib.addCSourceFiles(.{
    .files = &.{
      "src/Hello.cc",
      "swig_wrap.cxx",
    },
    .flags = &.{
      "-std=c++20",
      "-fexperimental-library",
      "-Wall",
      "-Wextra",
      "-Wpedantic",
    },
  });
  lib.linkLibC();
  lib.linkLibCpp();
  b.installArtifact(lib);
}
