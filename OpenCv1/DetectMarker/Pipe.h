#pragma once
#pragma region Includes
#include <stdio.h>
#include <windows.h>
#include <sddl.h>
#include <string>
#include <stdio.h>  
#include <memory>
#include <vector>

#pragma endregion
// The full name of the pipe in the format of \\servername\pipe\pipename.
#define SERVER_NAME         "."
#define PIPE_NAME           "test-pipe"
#define FULL_PIPE_NAME      "\\\\" SERVER_NAME "\\pipe\\" PIPE_NAME
#define OUT_BUFFER_SIZE     1024
#define BUFFER_SIZE     1024

// Response message from client to server. '\0' is appended in the end 
// because the client may be a native C++ application that expects NULL 
// termiated string.
#define RESPONSE_MESSAGE    "Default response from server"
BOOL CreatePipeSecurity(PSECURITY_ATTRIBUTES *);
void FreePipeSecurity(PSECURITY_ATTRIBUTES);
using namespace std;

class Pipe
{
public:
	Pipe();
	int CreateSecurity();
	int Cleanup();
	int CreatePipe();
	int Connect();
	DWORD Read(string & retString);
	DWORD Write(string outString);
	int Disconnect();
private:

	DWORD dwError = ERROR_SUCCESS;
	PSECURITY_ATTRIBUTES pSa = NULL;
	HANDLE hNamedPipe = INVALID_HANDLE_VALUE;

};
