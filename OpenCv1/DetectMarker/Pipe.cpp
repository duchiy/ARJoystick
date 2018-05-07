#include "Pipe.h"
using namespace std;
Pipe::Pipe()
{

};
int Pipe::CreateSecurity()
{

// Prepare the security attributes (the lpSecurityAttributes parameter in 
// CreateNamedPipe) for the pipe. This is optional. If the 
// lpSecurityAttributes parameter of CreateNamedPipe is NULL, the named 
// pipe gets a default security descriptor and the handle cannot be 
// inherited. The ACLs in the default security descriptor of a pipe grant 
// full control to the LocalSystem account, (elevated) administrators, 
// and the creator owner. They also give only read access to members of 
// the Everyone group and the anonymous account. However, if you want to 
// customize the security permission of the pipe, (e.g. to allow 
// Authenticated Users to read from and write to the pipe), you need to 
// create a SECURITY_ATTRIBUTES structure.
if (!CreatePipeSecurity(&pSa))
{
	dwError = GetLastError();
	printf("CreatePipeSecurity failed w/err 0x%08lx\n", dwError);
	Cleanup();
}
return 0;
}
int Pipe::Cleanup()
{
// Centralized cleanup for all allocated resources.
if (pSa != NULL)
{
	FreePipeSecurity(pSa);
	pSa = NULL;
}
if (hNamedPipe != INVALID_HANDLE_VALUE)
{
	CloseHandle(hNamedPipe);
	hNamedPipe = INVALID_HANDLE_VALUE;
}
return 0;
}

int Pipe::CreatePipe()
{

   // Create the named pipe.
    hNamedPipe = CreateNamedPipe(
	FULL_PIPE_NAME,             // Pipe name.
	PIPE_ACCESS_DUPLEX,         // The pipe is duplex; both server and 
								// client processes can read from and 
								// write to the pipe
	PIPE_TYPE_MESSAGE |         // Message type pipe 
	PIPE_READMODE_MESSAGE |     // Message-read mode 
	PIPE_WAIT,                  // Blocking mode is enabled
	PIPE_UNLIMITED_INSTANCES,   // Max. instances
	BUFFER_SIZE,                // Output buffer size in bytes
	BUFFER_SIZE,                // Input buffer size in bytes
	NMPWAIT_USE_DEFAULT_WAIT,   // Time-out interval
	pSa                         // Security attributes
	);

	if (hNamedPipe == INVALID_HANDLE_VALUE)
	{
		dwError = GetLastError();
		printf("Unable to create named pipe w/err 0x%08lx\n", dwError);
		Cleanup();
	}

	printf("The named pipe (%s) is created.\n", FULL_PIPE_NAME);

	// Wait for the client to connect.
	printf("Waiting for the client's connection...\n");

	return 0;

}

int Pipe::Connect()
{

	if (!ConnectNamedPipe(hNamedPipe, NULL))
	{
		if (ERROR_PIPE_CONNECTED != GetLastError())
		{
			dwError = GetLastError();
			wprintf(L"ConnectNamedPipe failed w/err 0x%08lx\n", dwError);
			Cleanup();
		}
	}
	printf("Client is connected.\n");

	return 0;
}
DWORD Pipe::Read(string & retString)
{

	BOOL fFinishRead = FALSE;
	DWORD cbRead = 0;
	char chRequest[BUFFER_SIZE];
	do
	{

		DWORD cbRequest;
		cbRequest = sizeof(chRequest);

		fFinishRead = ReadFile(
			hNamedPipe,     // Handle of the pipe
			chRequest,      // Buffer to receive data
			cbRequest,      // Size of buffer in bytes
			&cbRead,        // Number of bytes read
			NULL            // Not overlapped I/O
		);

		if (!fFinishRead && ERROR_MORE_DATA != GetLastError())
		{
			dwError = GetLastError();
			printf("ReadFile from pipe failed w/err 0x%08lx\n", dwError);
			Cleanup();
		}


	} while (!fFinishRead); // Repeat loop if ERROR_MORE_DATA
	string myString(chRequest);
	retString = myString.substr(0, cbRead);
	printf("Receive %ld bytes from client: \"%s\"\n", cbRead, retString.c_str());
	return cbRead;
}

DWORD Pipe::Write(string outString)
{
	// 
	// Send a response from server to client.
	// 
	int n = outString.length();
	char* char_array = new char[n];
	//		std::strcpy(char_array, outString.c_str());
	vector<char> vchar(outString.begin(), outString.end());
	vchar.push_back('\0');
	char* chResponse = &vchar[0];

	DWORD cbResponse, cbWritten;
	cbResponse = vchar.size() - 1;

	if (!WriteFile(
		hNamedPipe,     // Handle of the pipe
		chResponse,     // Buffer to write
		cbResponse,     // Number of bytes to write 
		&cbWritten,     // Number of bytes written 
		NULL            // Not overlapped I/O
	))
	{
		dwError = GetLastError();
		printf("WriteFile to pipe failed w/err 0x%08lx\n", dwError);
		Cleanup();
	}

	printf("Send %ld bytes to client: \"%s\"\n", cbWritten, chResponse);
	return 0;

}
int Pipe::Disconnect()
{
	// Flush the pipe to allow the client to read the pipe's contents 
	// before disconnecting. Then disconnect the client's connection. 
	FlushFileBuffers(hNamedPipe);
	DisconnectNamedPipe(hNamedPipe);
	return 0;
}
//
//   FUNCTION: CreatePipeSecurity(PSECURITY_ATTRIBUTES *)
//
//   PURPOSE: The CreatePipeSecurity function creates and initializes a new 
//   SECURITY_ATTRIBUTES structure to allow Authenticated Users read and 
//   write access to a pipe, and to allow the Administrators group full 
//   access to the pipe.
//
//   PARAMETERS:
//   * ppSa - output a pointer to a SECURITY_ATTRIBUTES structure that allows 
//     Authenticated Users read and write access to a pipe, and allows the 
//     Administrators group full access to the pipe. The structure must be 
//     freed by calling FreePipeSecurity.
//
//   RETURN VALUE: Returns TRUE if the function succeeds.
//
//   EXAMPLE CALL:
//
//     PSECURITY_ATTRIBUTES pSa = NULL;
//     if (CreatePipeSecurity(&pSa))
//     {
//         // Use the security attributes
//         // ...
//
//         FreePipeSecurity(pSa);
//     }
//
BOOL CreatePipeSecurity(PSECURITY_ATTRIBUTES *ppSa)
{
	BOOL fSucceeded = TRUE;
	DWORD dwError = ERROR_SUCCESS;

	PSECURITY_DESCRIPTOR pSd = NULL;
	PSECURITY_ATTRIBUTES pSa = NULL;

	// Define the SDDL for the security descriptor.
	PCSTR szSDDL = "D:"       // Discretionary ACL
		"(A;OICI;GRGW;;;AU)"   // Allow read/write to authenticated users
		"(A;OICI;GA;;;BA)";    // Allow full control to administrators

	if (!ConvertStringSecurityDescriptorToSecurityDescriptor(szSDDL,
		SDDL_REVISION_1, &pSd, NULL))
	{
		fSucceeded = FALSE;
		dwError = GetLastError();
		goto Cleanup;
	}

	// Allocate the memory of SECURITY_ATTRIBUTES.
	pSa = (PSECURITY_ATTRIBUTES)LocalAlloc(LPTR, sizeof(*pSa));
	if (pSa == NULL)
	{
		fSucceeded = FALSE;
		dwError = GetLastError();
		goto Cleanup;
	}

	pSa->nLength = sizeof(*pSa);
	pSa->lpSecurityDescriptor = pSd;
	pSa->bInheritHandle = FALSE;

	*ppSa = pSa;

Cleanup:
	// Clean up the allocated resources if something is wrong.
	if (!fSucceeded)
	{
		if (pSd)
		{
			LocalFree(pSd);
			pSd = NULL;
		}
		if (pSa)
		{
			LocalFree(pSa);
			pSa = NULL;
		}

		SetLastError(dwError);
	}

	return fSucceeded;
}


//
//   FUNCTION: FreePipeSecurity(PSECURITY_ATTRIBUTES)
//
//   PURPOSE: The FreePipeSecurity function frees a SECURITY_ATTRIBUTES 
//   structure that was created by the CreatePipeSecurity function. 
//
//   PARAMETERS:
//   * pSa - pointer to a SECURITY_ATTRIBUTES structure that was created by 
//     the CreatePipeSecurity function. 
//
void FreePipeSecurity(PSECURITY_ATTRIBUTES pSa)
{
	if (pSa)
	{
		if (pSa->lpSecurityDescriptor)
		{
			LocalFree(pSa->lpSecurityDescriptor);
		}
		LocalFree(pSa);
	}
}
