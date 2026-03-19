package com.layla.android.data.api

import com.layla.android.data.model.ProjectDto
import retrofit2.Response
import retrofit2.http.GET

interface ProjectApiService {
    @GET("api/projects/public")
    suspend fun getPublicProjects(): Response<List<ProjectDto>>
}
