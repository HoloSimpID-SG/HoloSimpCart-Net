const std = @import("std");

pub fn build(b: *std.Build) void {
    const target = b.standardTargetOptions(.{});
    const optimize = b.standardOptimizeOption(.{});
    const exe = b.addExecutable(.{
        .name = "HelloWorld",
        .root_module = b.createModule(.{
            .target = target,
            .optimize = optimize,
        }),
    });
    const zigs = b.addStaticLibrary(.{
        .name = "zig_lib",
        .root_source_file = b.path("src/util.zig"), // Path to a Zig file
        .target = target,
        .optimize = optimize,
    });
    zigs.linkLibC();
    zigs.linkLibCpp();
    b.installArtifact(zigs);

    exe.addCSourceFiles(.{
        .files = &.{
            "test.cc",
            "src/Hello.cc",
        },
        .flags = &.{
            "-std=c++20",
            "-fexperimental-library",
        },
    });
    exe.linkLibrary(zigs);
    exe.linkLibC();
    exe.linkLibCpp();
    b.installArtifact(exe);

    const run_cmd = b.addRunArtifact(exe);
    b.step("run", "Build and run").dependOn(&run_cmd.step);
}
