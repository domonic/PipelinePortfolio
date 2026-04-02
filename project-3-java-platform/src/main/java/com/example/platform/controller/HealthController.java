package com.example.platform.controller;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Map;

@RestController
public class HealthController {

    @Value("${spring.profiles.active:default}")
    private String activeProfile;

    @Value("${app.version:1.0.0}")
    private String appVersion;

    @GetMapping("/health")
    public Map<String, String> health() {
        return Map.of(
                "status", "healthy",
                "environment", activeProfile,
                "version", appVersion
        );
    }
}
