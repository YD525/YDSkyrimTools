// pch.h: 这是预编译标头文件。
// 下方列出的文件仅编译一次，提高了将来生成的生成性能。
// 这还将影响 IntelliSense 性能，包括代码完成和许多代码浏览功能。
// 但是，如果此处列出的文件中的任何一个在生成之间有更新，它们全部都将被重新编译。
// 请勿在此处添加要频繁更新的文件，这将使得性能优势无效。

#ifndef PCH_H
#define PCH_H

// 添加要在此处预编译的标头
#include "framework.h"
#include<windows.h>
#include <psapi.h> // For access to GetModuleFileNameEx
#include <tchar.h>

#include <iostream>

using namespace std;

#ifdef _UNICODE
#define tcout wcout
#define tcerr wcerr
#else
#define tcout cout
#define tcerr cerr
#endif

#endif //PCH_H

extern "C"
{
	_declspec(dllexport) int AnySendMessage(HWND ThisHwnd, LPCWSTR Char,BOOL ShowThis);
	_declspec(dllexport) int AnySendCrlf(HWND ThisHwnd,BOOL ShowThis);
	_declspec(dllexport) int AnySendTab(HWND ThisHwnd, BOOL ShowThis);
	_declspec(dllexport) int AnySendDown(HWND ThisHwnd);
	_declspec(dllexport) int AnySendLeft(HWND ThisHwnd, BOOL ShowThis);
	_declspec(dllexport) int AnySendRight(HWND ThisHwnd, BOOL ShowThis);
	_declspec(dllexport) int AnySendUP(HWND ThisHwnd);
	_declspec(dllexport) int AnySendESC(HWND ThisHwnd);
	_declspec(dllexport) int AnySendEnter(HWND ThisHwnd);
	_declspec(dllexport) int AnyAutoDelBlockText(HWND ThisHwnd,INT Count, BOOL ShowThis);
	_declspec(dllexport) int AnyAutoDelBlockTextR(HWND ThisHwnd, INT Count, INT SSleep);
	_declspec(dllexport) int SelectAll(HWND ThisHwnd, BOOL ShowThis, INT X, INT Y);
	_declspec(dllexport) int ClickTarget(HWND ThisHwnd, BOOL ShowThis, INT X, INT Y);
	_declspec(dllexport) int ClickTargetR(HWND ThisHwnd,INT X, INT Y);
	_declspec(dllexport) int ClickTargetRR(HWND ThisHwnd, INT X, INT Y);
	_declspec(dllexport) int ClickTargetByRight(HWND ThisHwnd, BOOL ShowThis, INT X, INT Y);
	_declspec(dllexport) BOOLEAN KrnGetProcessPath(DWORD hProcessId, OUT LPSTR ReturnPath);
	_declspec(dllexport) int AnySendF6(HWND ThisHwnd, BOOL ShowThis);
	_declspec(dllexport) int AnySendTabR(HWND ThisHwnd);
	_declspec(dllexport) int AnySendTabRR(HWND ThisHwnd);
	_declspec(dllexport) int AnySendMessageR(HWND ThisHwnd, LPCWSTR Char);
	_declspec(dllexport) int DoubleClickTarget(HWND ThisHwnd, BOOL ShowThis, INT X, INT Y);
	_declspec(dllexport) int DoubleClickTargetR(HWND ThisHwnd,INT X, INT Y);
	_declspec(dllexport) int AnySendCtrlA(HWND ThisHwnd);
	_declspec(dllexport) int AnySendDownR(HWND ThisHwnd);
	_declspec(dllexport) int SendCtrlAndEnter(HWND ThisHwnd);
	_declspec(dllexport) int ClickTargetMy(HWND ThisHwnd, INT X, INT Y);
	_declspec(dllexport) int DoubleClickTargetMy(HWND ThisHwnd, INT X, INT Y);
};
