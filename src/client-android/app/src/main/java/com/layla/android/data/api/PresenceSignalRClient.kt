package com.layla.android.data.api

import com.microsoft.signalr.HubConnection
import com.microsoft.signalr.HubConnectionBuilder
import com.microsoft.signalr.HubConnectionState
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow

data class ProjectPresence(val projectId: String, val isActive: Boolean)

class PresenceSignalRClient(private val baseUrl: String) {

    private var hubConnection: HubConnection? = null

    private val _connectionState = MutableStateFlow(ConnectionState.DISCONNECTED)
    val connectionState: StateFlow<ConnectionState> = _connectionState.asStateFlow()

    private val _presenceUpdates = MutableStateFlow<ProjectPresence?>(null)
    val presenceUpdates: StateFlow<ProjectPresence?> = _presenceUpdates.asStateFlow()

    fun connect(token: String? = null) {
        if (hubConnection != null) return

        _connectionState.value = ConnectionState.CONNECTING

        val url = if (token != null) {
            "$baseUrl/hubs/presence?access_token=$token"
        } else {
            "$baseUrl/hubs/presence"
        }

        hubConnection = HubConnectionBuilder.create(url).build()

        registerHandlers()

        hubConnection?.onClosed {
            _connectionState.value = ConnectionState.DISCONNECTED
        }

        hubConnection?.start()?.blockingAwait()
        _connectionState.value = ConnectionState.CONNECTED
    }

    fun watchProject(projectId: String) {
        if (hubConnection?.connectionState == HubConnectionState.CONNECTED) {
            hubConnection?.invoke("WatchProject", projectId)
        }
    }

    fun unwatchProject(projectId: String) {
        if (hubConnection?.connectionState == HubConnectionState.CONNECTED) {
            hubConnection?.invoke("UnwatchProject", projectId)
        }
    }

    fun sendHeartbeat(projectId: String) {
        if (hubConnection?.connectionState == HubConnectionState.CONNECTED) {
            hubConnection?.invoke("AuthorHeartbeat", projectId)
        }
    }

    fun disconnect() {
        hubConnection?.stop()?.blockingAwait()
        hubConnection = null
        _connectionState.value = ConnectionState.DISCONNECTED
    }

    private fun registerHandlers() {
        hubConnection?.on("AuthorStatusChanged", { projectId: String, isActive: Boolean ->
            _presenceUpdates.value = ProjectPresence(projectId, isActive)
        }, String::class.java, Boolean::class.java)
    }
}
