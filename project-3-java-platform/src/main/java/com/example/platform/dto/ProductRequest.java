package com.example.platform.dto;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Positive;
import java.math.BigDecimal;

public record ProductRequest(
        @NotBlank String name,
        String description,
        @Positive BigDecimal price,
        @NotBlank String category
) {}
