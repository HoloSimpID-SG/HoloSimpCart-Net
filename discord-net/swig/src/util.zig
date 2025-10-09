const std = @import("std");

pub export fn zig_hello(name: [*:0]const u8, number: i32) callconv(.C) ?[*:0]u8 {
  const allocator = std.heap.c_allocator;

  const formatted = std.fmt.allocPrint(allocator, "{s} {d} (By Zig)", .{ name, number }) catch return null;
  const ptr_with_sentinel: [*:0]u8 = @ptrCast(formatted.ptr);

  return ptr_with_sentinel;
}
