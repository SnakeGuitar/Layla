package com.layla.android.data.model

data class ProjectDto(
    val id: String,
    val title: String,
    val synopsis: String,
    val literaryGenre: String,
    val coverImageUrl: String?,
    val updatedAt: String,
    val isPublic: Boolean,
    val userRole: String = ""
)
