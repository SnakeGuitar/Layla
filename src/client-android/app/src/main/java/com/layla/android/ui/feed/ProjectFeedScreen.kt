package com.layla.android.ui.feed

import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import androidx.lifecycle.viewModelScope
import com.layla.android.data.api.PresenceSignalRClient
import com.layla.android.data.api.ProjectApiService
import com.layla.android.data.model.ProjectDto
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch


data class FeedProject(
    val dto: ProjectDto,
    val isAuthorActive: Boolean = false
)

sealed class FeedState {
    object Loading : FeedState()
    data class Success(val projects: List<FeedProject>) : FeedState()
    data class Error(val message: String) : FeedState()
}

class ProjectFeedViewModel(
    private val projectApiService: ProjectApiService,
    private val presenceClient: PresenceSignalRClient,
    private val token: String?
) : ViewModel() {

    private val _feedState = MutableStateFlow<FeedState>(FeedState.Loading)
    val feedState: StateFlow<FeedState> = _feedState.asStateFlow()

    init {
        loadFeed()
        observePresence()
    }

    private fun loadFeed() {
        viewModelScope.launch {
            try {
                val response = projectApiService.getPublicProjects()
                if (response.isSuccessful) {
                    val projects = response.body()?.map { FeedProject(it) } ?: emptyList()
                    _feedState.value = FeedState.Success(projects)
                    connectPresence(projects.map { it.dto.id })
                } else {
                    _feedState.value = FeedState.Error("Failed to load projects")
                }
            } catch (e: Exception) {
                _feedState.value = FeedState.Error(e.message ?: "Unknown error")
            }
        }
    }

    private fun connectPresence(projectIds: List<String>) {
        viewModelScope.launch(Dispatchers.IO) {
            try {
                presenceClient.connect(token)
                projectIds.forEach { presenceClient.watchProject(it) }
            } catch (e: Exception) {
            }
        }
    }

    private fun observePresence() {
        viewModelScope.launch {
            presenceClient.presenceUpdates.collect { update ->
                update ?: return@collect
                val current = _feedState.value
                if (current is FeedState.Success) {
                    val updated = current.projects.map { fp ->
                        if (fp.dto.id == update.projectId) fp.copy(isAuthorActive = update.isActive)
                        else fp
                    }
                    _feedState.value = FeedState.Success(updated)
                }
            }
        }
    }

    override fun onCleared() {
        super.onCleared()
        presenceClient.disconnect()
    }
}

class ProjectFeedViewModelFactory(
    private val projectApiService: ProjectApiService,
    private val presenceClient: PresenceSignalRClient,
    private val token: String?
) : ViewModelProvider.Factory {
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        @Suppress("UNCHECKED_CAST")
        return ProjectFeedViewModel(projectApiService, presenceClient, token) as T
    }
}

@Composable
fun ProjectFeedScreen(
    viewModel: ProjectFeedViewModel,
    onProjectClick: (String) -> Unit = {}
) {
    val state by viewModel.feedState.collectAsState()

    Column(
        modifier = Modifier
            .fillMaxSize()
            .padding(horizontal = 16.dp)
    ) {
        Spacer(modifier = Modifier.height(24.dp))

        Text(
            text = "PUBLIC LIBRARY",
            fontSize = 10.sp,
            fontWeight = FontWeight.Bold,
            color = Color(0xFFF59E0B),
            letterSpacing = 3.sp
        )

        Spacer(modifier = Modifier.height(4.dp))

        Text(
            text = "Projects",
            style = MaterialTheme.typography.headlineMedium,
            color = Color(0xFFF5F5F4)
        )

        HorizontalDivider(
            modifier = Modifier
                .width(64.dp)
                .padding(top = 8.dp, bottom = 20.dp),
            color = Color(0xFFF59E0B),
            thickness = 1.dp
        )

        when (state) {
            is FeedState.Loading -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    CircularProgressIndicator(color = Color(0xFFF59E0B))
                }
            }
            is FeedState.Error -> {
                Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                    Text(
                        text = (state as FeedState.Error).message,
                        color = Color(0xFF78716C)
                    )
                }
            }
            is FeedState.Success -> {
                val projects = (state as FeedState.Success).projects
                if (projects.isEmpty()) {
                    Box(modifier = Modifier.fillMaxSize(), contentAlignment = Alignment.Center) {
                        Text(text = "No public projects yet.", color = Color(0xFF78716C))
                    }
                } else {
                    LazyColumn(verticalArrangement = Arrangement.spacedBy(1.dp)) {
                        items(projects) { fp ->
                            ProjectCard(
                                id = fp.dto.id,
                                title = fp.dto.title,
                                synopsis = fp.dto.synopsis,
                                literaryGenre = fp.dto.literaryGenre,
                                isAuthorActive = fp.isAuthorActive,
                                onClick = onProjectClick
                            )
                        }
                    }
                }
            }
        }
    }
}
