// pch.cpp: 与预编译标头对应的源文件

#include "pch.h"

// 当使用预编译的头时，需要使用此源文件，编译才能成功。

//0x0100 KeyDown 0x0101 KeyUP

int MakeKeyLparam(int VirtualKey)
{
	UINT sCode;

	UINT iKey = MapVirtualKey(VirtualKey, 0);

	sCode = (iKey << 16) + 1;

	return sCode;
}

int AnySendDownR(HWND ThisHwnd)
{
	try
	{
		SetForegroundWindow(ThisHwnd);

		Sleep(5);

		PostMessage(ThisHwnd, 0x0100, VK_DOWN, MakeKeyLparam(VK_SPACE));

		Sleep(5);

		PostMessage(ThisHwnd, 0x0101, VK_DOWN, MakeKeyLparam(VK_SPACE));

		Sleep(5);

		return 1;
	}
	catch (char* Error) {
		return -1;
	}
}

int AnySendMessage(HWND ThisHwnd, LPCWSTR Char, BOOL ShowThis)
{
	try
	{

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);
		Sleep(50);

		for (int i = 0; i < lstrlen(Char); i++)
		{
		   PostMessage(ThisHwnd, WM_CHAR, Char[i], 0);
		}
		Sleep(10);

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	catch (char *Error) {
		return -1;
	}

}

int AnySendMessageR(HWND ThisHwnd, LPCWSTR Char)
{
	try
	{
		Sleep(1);

		for (int i = 0; i < lstrlen(Char); i++)
		{
			PostMessage(ThisHwnd, WM_CHAR, Char[i], 0);
		}

		return 0;
	}
	catch (char* Error) {
		return -1;
	}

}

int AnySendDown(HWND ThisHwnd)
{
	try
	{
		Sleep(1);

		PostMessage(ThisHwnd, 0x0100, VK_DOWN, NULL);
		PostMessage(ThisHwnd, 0x0101, VK_DOWN, NULL);
		
	    return 0;
	}
	catch (char* Error) {
		return -1;
	}

}

int AnySendLeft(HWND ThisHwnd, BOOL ShowThis)
{
	try
	{
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);

		PostMessage(ThisHwnd, 0x0100, VK_LEFT, NULL);
		PostMessage(ThisHwnd, 0x0101, VK_LEFT, NULL);
		Sleep(5);

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	catch (char* Error) {
		return -1;
	}

}

int AnySendRight(HWND ThisHwnd, BOOL ShowThis)
{
	try
	{
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);

		PostMessage(ThisHwnd, 0x0100, VK_RIGHT, NULL);
		PostMessage(ThisHwnd, 0x0101, VK_RIGHT, NULL);
		Sleep(5);

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	catch (char* Error) {
		return -1;
	}

}

int AnySendUP(HWND ThisHwnd)
{
	try
	{
		Sleep(1);

		PostMessage(ThisHwnd, 0x0100, VK_UP, NULL);
		PostMessage(ThisHwnd, 0x0101, VK_UP, NULL);

        return 0;	
	}
	catch (char* Error) {
		return -1;
	}

}

int AnySendESC(HWND ThisHwnd)
{
	try
	{
		Sleep(1);

		PostMessage(ThisHwnd, 0x0100, VK_ESCAPE, NULL);
		PostMessage(ThisHwnd, 0x0101, VK_ESCAPE, NULL);
		
	    return 0;
	}
	catch (char* Error) {
		return -1;
	}

}

int AnySendTabR(HWND ThisHwnd)
{
	try
	{
		PostMessage(ThisHwnd, 0x0100, VK_TAB, NULL);
		Sleep(1);
		PostMessage(ThisHwnd, 0x0101, VK_TAB, NULL);
		Sleep(5);

		return 0;
	}
	catch (char* Error) {
		return -1;
	}

}

int AnySendTabRR(HWND ThisHwnd)
{
	try
	{
		PostMessage(ThisHwnd, 0x0100, VK_TAB, NULL);

		return 0;
	}
	catch (char* Error) {
		return -1;
	}
}

int AnySendTab(HWND ThisHwnd, BOOL ShowThis)
{
	try
	{
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);

		PostMessage(ThisHwnd, 0x0100, VK_TAB, NULL);
		PostMessage(ThisHwnd, 0x0101, VK_TAB, NULL);
		Sleep(5);

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	catch (char *Error) {
		return -1;
	}

}

int AnySendCrlf(HWND ThisHwnd, BOOL ShowThis)
{
	try
	{
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);

		PostMessage(ThisHwnd, 0x0100, 13, 0);
		PostMessage(ThisHwnd, 0x0101, 13, 0);
		Sleep(5);

		if (ShowThis == false)
		{
			Sleep(30);
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	catch (char *Error) {
		return -1;
	}

}

int AnySendEnter(HWND ThisHwnd)
{
	try
	{
		Sleep(1);
		PostMessage(ThisHwnd, 0x0100, VK_RETURN, NULL);
		PostMessage(ThisHwnd, 0x0101, VK_RETURN, NULL);
		return 0;
	}
	catch (char *Error) {
		return -1;
	}
}

int AnyAutoDelBlockTextR(HWND ThisHwnd, INT Count,INT SSleep)
{
	try
	{
		SetForegroundWindow(ThisHwnd);

		for (int i = 0; i < Count; i++)
		{
			PostMessage(ThisHwnd, 0x0100, 8, NULL);
			PostMessage(ThisHwnd, 0x0101, 8, NULL);
			Sleep(SSleep);
		}

		return 0;
	}
	catch (char* Error) {
		return -1;
	}
}

int AnyAutoDelBlockText(HWND ThisHwnd, INT Count, BOOL ShowThis)
{
	try
	{
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);

		for (int i = 0; i < Count; i++)
		{
			PostMessage(ThisHwnd, 0x0100, 8, NULL);
			PostMessage(ThisHwnd, 0x0101, 8, NULL);
			Sleep(1);
		}

		
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	catch (char *Error) {
		return -1;
	}
}

int AnySendF6(HWND ThisHwnd, BOOL ShowThis)
{
	try
	{

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);

		PostMessage(ThisHwnd, 0x0100, VK_F6, NULL);
		PostMessage(ThisHwnd, 0x0101, VK_F6, NULL);
		Sleep(5);

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	catch (char* Error) {
		return -1;
	}
}

int DoubleClickTargetR(HWND ThisHwnd,INT X, INT Y)
{
	try
	{
		//PostMessage(ThisHwnd, WM_SETCURSOR, 1, MAKELPARAM(0, 0));
		//PostMessage(ThisHwnd, WM_MOUSEMOVE, 1, MAKELPARAM(0, 0));
		PostMessage(ThisHwnd, WM_MOUSEMOVE, 0, MAKELPARAM(X, Y));
		PostMessage(ThisHwnd, WM_MBUTTONDBLCLK, 0, MAKELPARAM(X, Y));
		/*Sleep(5);
		PostMessage(ThisHwnd, WM_LBUTTONUP, 0, X + Y * 65536);*/

		return 0;
	}
	catch (char* Error) {
		return -1;
	}
}

int ClickTargetR(HWND ThisHwnd,INT X, INT Y)
{
	try
	{
		//PostMessage(ThisHwnd, WM_SETCURSOR, 1, MAKELPARAM(0, 0));
		//PostMessage(ThisHwnd, WM_MOUSEMOVE, 1, MAKELPARAM(0, 0));
		PostMessage(ThisHwnd, WM_MOUSEMOVE, 1, MAKELPARAM(X, Y));
		PostMessage(ThisHwnd, WM_LBUTTONDOWN, 1, MAKELPARAM(X, Y));
		Sleep(5);
		PostMessage(ThisHwnd, WM_LBUTTONUP, 1, MAKELPARAM(X, Y));
		
		/*Sleep(5);
		PostMessage(ThisHwnd, WM_LBUTTONUP, 0, X + Y * 65536);*/

		return 0;
	}
	catch (char* Error) {
		return -1;
	}
}

int ClickTargetMy(HWND ThisHwnd, INT X, INT Y)
{
	try
	{
		PostMessage(ThisHwnd, WM_LBUTTONDOWN, 1, MAKELPARAM(X, Y));
		PostMessage(ThisHwnd, WM_LBUTTONUP, 1, MAKELPARAM(X, Y));
		return 0;
	}
	catch (char* Error) {
		return -1;
	}
}

int DoubleClickTargetMy(HWND ThisHwnd,INT X, INT Y)
{
	try
	{
		PostMessage(ThisHwnd, WM_MBUTTONDBLCLK, 0, MAKELPARAM(X, Y));
	}
	catch (char* Error) {
		return -1;
	}

}

int ClickTargetRR(HWND ThisHwnd, INT X, INT Y)
{
	try
	{
		//PostMessage(ThisHwnd, WM_SETCURSOR, 1, MAKELPARAM(0, 0));
		//PostMessage(ThisHwnd, WM_MOUSEMOVE, 1, MAKELPARAM(0, 0));

		PostMessage(ThisHwnd, WM_MOUSEMOVE, 0, MAKELPARAM(X, Y));
		Sleep(2);
		PostMessage(ThisHwnd, WM_LBUTTONDOWN, MK_LBUTTON, MAKELPARAM(X, Y));
		PostMessage(ThisHwnd, WM_LBUTTONUP, MK_LBUTTON, MAKELPARAM(X, Y));

		/*Sleep(5);
		PostMessage(ThisHwnd, WM_LBUTTONUP, 0, X + Y * 65536);*/

		return 0;
	}
	catch (char* Error) {
		return -1;
	}
}

int ClickTarget(HWND ThisHwnd, BOOL ShowThis, INT X, INT Y)
{
	try
	{
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);
		
		PostMessage(ThisHwnd, WM_LBUTTONDOWN, 0, MAKELPARAM(X, Y));
		PostMessage(ThisHwnd, WM_LBUTTONUP, 0, MAKELPARAM(X, Y));
		Sleep(5);

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}

	}
	catch (char *Error) {
		return -1;
	}

}

int ClickTargetByRight(HWND ThisHwnd, BOOL ShowThis, INT X, INT Y)
{
	try
	{
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);
		PostMessage(ThisHwnd, WM_MOUSEMOVE, 0, MAKELPARAM(X, Y));
		Sleep(5);
		PostMessage(ThisHwnd, WM_RBUTTONDOWN, 0, MAKELPARAM(X, Y));
		Sleep(5);
		PostMessage(ThisHwnd, WM_RBUTTONUP, 0, MAKELPARAM(X, Y));
		Sleep(5);

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}

	}
	catch (char* Error) {
		return -1;
	}
}

int DoubleClickTarget(HWND ThisHwnd, BOOL ShowThis, INT X, INT Y)
{
	try
	{
		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		SetForegroundWindow(ThisHwnd);
		
		//wm mb button double click
		PostMessage(ThisHwnd, WM_MBUTTONDBLCLK, 0, MAKELPARAM(X, Y));
		

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}

	}
	catch (char* Error) {
		return -1;
	}

}

int AnySendCtrlA(HWND ThisHwnd)
{
	try
	{
	PostMessage(ThisHwnd, WM_KEYDOWN, VK_CONTROL, 0);
	PostMessage(ThisHwnd, WM_KEYDOWN, 'A', 0);
	Sleep(30);
	PostMessage(ThisHwnd, WM_KEYUP, VK_CONTROL, 0);
	PostMessage(ThisHwnd, WM_KEYUP, 'A', 0);

	return 0;
	}
	catch (char* Error) {
		return -1;
	}
}

int SendCtrlAndEnter(HWND ThisHwnd)
{
	try
	{
    Sleep(15);
	keybd_event(VK_CONTROL,MapVirtualKey(VK_CONTROL,0),0,0);
	Sleep(5);
	PostMessage(ThisHwnd, WM_KEYDOWN, VK_RETURN, 0);
	Sleep(20);
	keybd_event(VK_CONTROL, MapVirtualKey(VK_CONTROL, 0), KEYEVENTF_KEYUP, 0);
	PostMessage(ThisHwnd, WM_KEYUP, VK_RETURN, 0);
	return 0;
	}
	catch (char* Error) {
		return -1;
	}
}

int SelectAll(HWND ThisHwnd, BOOL ShowThis, INT X, INT Y)
{
	try
	{
		SetForegroundWindow(ThisHwnd);

		Sleep(30);

		if (ShowThis == false)
		{
			ShowWindow(ThisHwnd, SW_SHOWNORMAL);
		}

		Sleep(30);

		PostMessage(ThisHwnd, WM_KEYDOWN, VK_CONTROL, 0x001D0001);
		PostMessage(ThisHwnd, WM_KEYDOWN, 'A', 0x001E0001);

		Sleep(30);

		PostMessage(ThisHwnd, WM_KEYUP, VK_CONTROL, 0xC01D0001);
		PostMessage(ThisHwnd, WM_KEYUP, 'A', 0xC01E0001);

		Sleep(30);

		PostMessage(ThisHwnd, WM_KEYDOWN, VK_CONTROL, 0x001D0001);
		PostMessage(ThisHwnd, WM_KEYDOWN, 'C', 0x001E0001);

		Sleep(30);

		PostMessage(ThisHwnd, WM_KEYUP, VK_CONTROL, 0xC01D0001);
		PostMessage(ThisHwnd, WM_KEYUP, 'C', 0xC01E0001);

		Sleep(30);

		PostMessage(ThisHwnd, WM_LBUTTONDOWN, 0, MAKELPARAM(X, Y));
		PostMessage(ThisHwnd, WM_LBUTTONUP, 0, MAKELPARAM(X, Y));

		if (ShowThis == false)
		{
			Sleep(30);
			ShowWindow(ThisHwnd, SW_MINIMIZE);
		}

		if (ShowThis == false)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
	catch (char *Error) {
		return -1;
	}
}

string TCHAR2STRING(TCHAR* str)
{
	string strstr;

	try
	{
		int iLen = WideCharToMultiByte(CP_ACP, 0, str, -1, NULL, 0, NULL, NULL);

		char* chRtn = new char[iLen * sizeof(char)];

		WideCharToMultiByte(CP_ACP, 0, str, -1, chRtn, iLen, NULL, NULL);

		strstr = chRtn;
	}
	catch (exception Ex)
	{
	}

	return strstr;
}

LPCWSTR stringToLPCWSTR(std::string orig)
{
	size_t origsize = orig.length() + 1;
	const size_t newsize = 100;
	size_t convertedChars = 0;
	wchar_t *wcstring = (wchar_t *)malloc(sizeof(wchar_t)*(orig.length() - 1));
	mbstowcs_s(&convertedChars, wcstring, origsize, orig.c_str(), _TRUNCATE);

	return wcstring;
}

string WCharToMByte(LPCWSTR lpcwszStr)
{
	string str;
	DWORD dwMinSize = 0;
	LPSTR lpszStr = NULL;
	dwMinSize = WideCharToMultiByte(CP_OEMCP, NULL, lpcwszStr, -1, NULL, 0, NULL, FALSE);
	if (0 == dwMinSize)
	{
		return FALSE;
	}
	lpszStr = new char[dwMinSize];
	WideCharToMultiByte(CP_OEMCP, NULL, lpcwszStr, -1, lpszStr, dwMinSize, NULL, FALSE);
	str = lpszStr;
	delete[] lpszStr;
	return str;
}

BOOLEAN KrnGetProcessPath(DWORD hProcessId, OUT LPSTR ReturnPath)
{
	HANDLE ProcessHandle = NULL;

	ProcessHandle = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, hProcessId);

	if (ProcessHandle != NULL) 
	{	
		DWORD size = MAX_PATH;

		if (QueryFullProcessImageNameA(ProcessHandle, 0, ReturnPath, &size))
		{
			return true;
		}
		else
		{
			return false;
		}

		CloseHandle(ProcessHandle);
	}

	return false;
}




