// C Client Program

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <unistd.h>
#include <arpa/inet.h>

#define PORT_C_SERVER 8888
#define PORT_C_SHARP_CLIENT 8889

int main(int argc,char *argv[]) {
    if (argc != 2) {
        fprintf(stderr, "Usage: %s <C server IP>\n", argv[0]);
        exit(1);
    }

    const char *cServerIP = argv[1];

    int cSharpClientSocket, cServerSocket;

    char buffer[4000000];

    struct sockaddr_in cSharpClientAddr, cServerAddr;

    socklen_t addr_size = sizeof(struct sockaddr);

    // Create the C client (server) socket for C# client
    cServerSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (cServerSocket < 0) {
        perror("Error in socket creation");
        exit(1);
    }

    // Configure C client address structure for C# client
    cServerAddr.sin_family = AF_INET;
    cServerAddr.sin_port = htons(PORT_C_SHARP_CLIENT);
    cServerAddr.sin_addr.s_addr = INADDR_ANY;

    // Bind the socket for C# client
    if (bind(cServerSocket, (struct sockaddr*)&cServerAddr, sizeof(cServerAddr)) < 0) {
        perror("Error in binding");
        exit(1);
    }

    // Listen for incoming connections from C# client
    if (listen(cServerSocket, 1) != 0) {
        perror("Error in listening");
        exit(1);
    }

    // Accept a connection from C# client
    cSharpClientSocket = accept(cServerSocket, (struct sockaddr*)&cSharpClientAddr, &addr_size);
    if (cSharpClientSocket < 0) {
        perror("Error in accepting C# client");
        exit(1);
    }

    // Create a new socket to connect to the C server
    int cServerConnectSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (cServerConnectSocket < 0) {
        perror("Error in socket creation");
        exit(1);
    }

    // Configure C server address structure
    struct sockaddr_in cServerConnectAddr;
    cServerConnectAddr.sin_family = AF_INET;
    cServerConnectAddr.sin_port = htons(PORT_C_SERVER);
    cServerConnectAddr.sin_addr.s_addr = inet_addr(cServerIP);

    // Connect to the C server
    if (connect(cServerConnectSocket, (struct sockaddr*)&cServerConnectAddr, sizeof(cServerConnectAddr)) < 0) {
        perror("Error in connection to C server");
        exit(1);
    }

    // Loop for bidirectional communication
    while (1) {
        printf("Listening for C# client...\n");

        // Receive data from C# client
        ssize_t bytesRead = recv(cSharpClientSocket, buffer, sizeof(buffer), 0);
        if (bytesRead <= 0) {
            if (bytesRead == 0) {
                // Connection closed by C# client
                printf("Connection closed by C# client.\n");
            } else {
                perror("Error receiving data from C# client");
            }
            break;
        }
        buffer[bytesRead] = '\0'; // Null-terminate the received data
        printf("Data received from C# client: %s\n", buffer);

        // Forward the data to C server
        send(cServerConnectSocket, buffer, strlen(buffer), 0);
        memset(buffer, 0, sizeof(buffer));

        // Receive data from C server
        bytesRead = recv(cServerConnectSocket, buffer, sizeof(buffer), 0);
        if (bytesRead <= 0) {
            if (bytesRead == 0) {
                // Connection closed by C server
                printf("Connection closed by C server.\n");
            } else {
                perror("Error receiving data from C server");
            }
            break;
        }
        buffer[bytesRead] = '\0'; // Null-terminate the received data
        printf("Data received from C server: %s\n", buffer);

        // Forward the data to C# client
        send(cSharpClientSocket, buffer, strlen(buffer), 0);
        memset(buffer, 0, sizeof(buffer));
    }

    // Close the sockets
    close(cSharpClientSocket);
    close(cServerConnectSocket);
    close(cServerSocket);

    return 0;
}

