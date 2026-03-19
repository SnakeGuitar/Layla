package com.layla.android

import android.os.Bundle
import android.widget.Toast
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.layla.android.data.api.PresenceSignalRClient
import com.layla.android.data.api.RetrofitClient
import com.layla.android.data.local.SessionManager
import com.layla.android.data.repository.AuthRepository
import com.layla.android.ui.auth.AuthViewModel
import com.layla.android.ui.auth.AuthViewModelFactory
import com.layla.android.ui.auth.LoginScreen
import com.layla.android.ui.auth.RegisterScreen
import com.layla.android.ui.feed.ProjectFeedScreen
import com.layla.android.ui.feed.ProjectFeedViewModel
import com.layla.android.ui.feed.ProjectFeedViewModelFactory
import com.layla.android.ui.theme.LaylaAndroidTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        
        val sessionManager = SessionManager(this)
        val repository = AuthRepository(RetrofitClient.authApiService)
        val viewModelFactory = AuthViewModelFactory(repository, sessionManager)

        setContent {
            LaylaAndroidTheme {
                val navController = rememberNavController()
                val authViewModel: AuthViewModel = viewModel(factory = viewModelFactory)

                NavHost(navController = navController, startDestination = "login") {
                    composable("login") {
                        LoginScreen(
                            viewModel = authViewModel,
                            onNavigateToRegister = { navController.navigate("register") },
                            onLoginSuccess = {
                                Toast.makeText(this@MainActivity, "Login Successful!", Toast.LENGTH_SHORT).show()
                                navController.navigate("feed") {
                                    popUpTo("login") { inclusive = true }
                                }
                            }
                        )
                    }
                    composable("feed") {
                        val token = sessionManager.fetchAuthToken()
                        val feedViewModel: ProjectFeedViewModel = viewModel(
                            factory = ProjectFeedViewModelFactory(
                                projectApiService = RetrofitClient.projectApiService,
                                presenceClient = PresenceSignalRClient("http://10.0.2.2:7165"),
                                token = token
                            )
                        )
                        ProjectFeedScreen(viewModel = feedViewModel)
                    }
                    composable("register") {
                        RegisterScreen(
                            viewModel = authViewModel,
                            onNavigateToLogin = { navController.popBackStack() },
                            onRegisterSuccess = {
                                Toast.makeText(this@MainActivity, "Registration Successful! Please login.", Toast.LENGTH_SHORT).show()
                                navController.navigate("login") {
                                    popUpTo("register") { inclusive = true }
                                }
                            }
                        )
                    }
                }
            }
        }
    }
}
