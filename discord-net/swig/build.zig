const std = @import("std");

pub fn build(b: *std.Build) void {
  const target = b.standardTargetOptions(.{});
  const optimize = b.standardOptimizeOption(.{});
  const exe = b.addSharedLibrary(.{
    .name = "Native",
    .root_source_file = b.path("main.zig"),
    .target = target,
    .optimize = optimize,
  });
  exe.addCSourceFiles(.{
    .files = &.{
      "src/Hello.cc",
      "swig_wrap.cxx",
    },
    .flags = &.{
      "-std=c++20",
      "-fexperimental-library",
    },
  });
  exe.linkLibC();
  exe.linkLibCpp();
  b.installArtifact(exe);
}
