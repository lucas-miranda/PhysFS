﻿//#define JUST_PRINT_ERROR

using System;
using System.Reflection;
using System.Diagnostics;

namespace PhysFS.Test {
    public delegate bool TestFunction();

    class Program {
        static void Main(string[] args) {
            int testsCount = 0,
                failedCount = 0;

            Debug.WriteLine("Begin tests\n");

            Type testsType = typeof(Tests);
            MethodInfo[] methods = testsType.GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach (MethodInfo methodInfo in methods) {
                TestCaseAttribute testCaseAttr = methodInfo.GetCustomAttribute<TestCaseAttribute>();

                if (testCaseAttr == null) {
                    continue;
                }

                testsCount++;

                TestFunction function = (TestFunction) methodInfo.CreateDelegate(typeof(TestFunction));
                if (!ExecuteTest(testCaseAttr.Title, function)) {
                    failedCount++;
                }
            }

            Debug.WriteLine($"\nEnd of tests");

            if (failedCount > 0) {
                Debug.WriteLine($"* Failed: {failedCount} of {testsCount}.");
            } else {
                Debug.WriteLine("* All tests succeed!");
            }
        }

        private static bool ExecuteTest(string title, TestFunction function) {
            Debug.WriteLine($"\n> {title}");

            bool ret = false;

#if JUST_PRINT_ERROR
            try {
                ret = function();
                Debug.WriteLine("  Passed!");
            } catch (PhysFSException e) {
                Debug.WriteLine($"  Failed\n  Error: [{e.ErrorCode}] {e.Message}");
            }
#else
            ret = function();
            if (ret) {
                Debug.WriteLine("Passed!");
            } else {
                Debug.WriteLine($"Failed");
            }
#endif

            return ret;
        }
    }
}
