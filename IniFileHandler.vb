Imports System.Runtime.InteropServices

Friend Class IniFileHandler
    Declare Function GetPrivateProfileString Lib "KERNEL32.DLL" Alias "GetPrivateProfileStringA" (
      <MarshalAs(UnmanagedType.LPStr)> ByVal lpAppName As String,
      <MarshalAs(UnmanagedType.LPStr)> ByVal lpKeyName As String,
      <MarshalAs(UnmanagedType.LPStr)> ByVal lpDefault As String,
      <MarshalAs(UnmanagedType.LPStr)> ByVal lpReturnedString As StringBuilder,
     ByVal nSize As Integer,
     <MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String) As Integer
End Class
