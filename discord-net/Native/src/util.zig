const std = @import("std");

fn reverseSlice(slice: []u8) void {
    var i: usize = 0;
    var j: usize = slice.len - 1;
    while (i < j) {
        std.mem.swap(u8, &slice[i], &slice[j]);
        i += 1;
        j -= 1;
    }
}

// Dont call this yet
// it crashes the bot
// I still don't undertand strings in zig
pub fn zig_hello(name: [*:0]const u8, number: i32) callconv(.C) [*:0]u8 {
    const allocator = std.heap.c_allocator;
    const input = std.mem.span(name);
    const result = allocator.allocSentinel(u8, input.len + 50, 0) catch unreachable;
    var i: usize = 0;
    while (i < input.len) : (i += 1) {
        result[i] = input[input.len - 1 - i];
    }
    const formatted = std.fmt.bufPrintZ(result[input.len..], "{d} (by Zig)", .{number}) catch unreachable;

    return formatted.ptr;
}

pub fn zig_num(number: i32) callconv(.C) i32 {
    const rand = std.crypto.random.intRangeAtMost(i32, -number, number);
    return rand;
}
