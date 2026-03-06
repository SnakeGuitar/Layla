# Available HTTPS endpoints

## Users
* POST      https://{Layla.Api}/api/Users

## Projects 
* POST      https://{Layla.Api}/api/Projects
    * BodyRequest: {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MaxLength(1000)]
        public string Synopsis { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string LiteraryGenre { get; set; } = string.Empty;
        [MaxLength(2048)]
        public string? CoverImageUrl { get; set; }
    }
    * BodyResponse: {
        
    }
* GET       https://{Layla.Api}/api/Projects
* GET       https://{Layla.Api}/api/Projects/{id}
* PUT       https://{Layla.Api}/api/Projects/{id}
* DELETE    https://{Layla.Api}/api/Projects/{id}

## Tokens
* POST      https://{Layla.Api}/api/Tokens

