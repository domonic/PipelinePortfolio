package com.example.platform.controller;

import com.example.platform.dto.ProductRequest;
import com.example.platform.dto.ProductResponse;
import com.example.platform.service.ProductService;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.mock.bean.MockBean;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;

import java.math.BigDecimal;
import java.time.Instant;
import java.util.List;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

@WebMvcTest(ProductController.class)
class ProductControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @Autowired
    private ObjectMapper objectMapper;

    @MockBean
    private ProductService productService;

    @Test
    void shouldListProducts() throws Exception {
        var product = new ProductResponse(1L, "Widget", "A widget", BigDecimal.TEN, "tools", Instant.now());
        when(productService.findAll()).thenReturn(List.of(product));

        mockMvc.perform(get("/api/products"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].name").value("Widget"));
    }

    @Test
    void shouldCreateProduct() throws Exception {
        var request = new ProductRequest("Widget", "A widget", BigDecimal.TEN, "tools");
        var response = new ProductResponse(1L, "Widget", "A widget", BigDecimal.TEN, "tools", Instant.now());
        when(productService.create(any())).thenReturn(response);

        mockMvc.perform(post("/api/products")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(request)))
                .andExpect(status().isCreated())
                .andExpect(jsonPath("$.name").value("Widget"));
    }

    @Test
    void shouldRejectInvalidProduct() throws Exception {
        var invalid = new ProductRequest("", null, BigDecimal.valueOf(-1), "");

        mockMvc.perform(post("/api/products")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content(objectMapper.writeValueAsString(invalid)))
                .andExpect(status().isBadRequest());
    }

    @Test
    void shouldGetProductById() throws Exception {
        var product = new ProductResponse(1L, "Widget", "A widget", BigDecimal.TEN, "tools", Instant.now());
        when(productService.findById(1L)).thenReturn(product);

        mockMvc.perform(get("/api/products/1"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.id").value(1));
    }

    @Test
    void shouldFilterByCategory() throws Exception {
        var product = new ProductResponse(1L, "Widget", "A widget", BigDecimal.TEN, "tools", Instant.now());
        when(productService.findByCategory("tools")).thenReturn(List.of(product));

        mockMvc.perform(get("/api/products").param("category", "tools"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].category").value("tools"));
    }
}
