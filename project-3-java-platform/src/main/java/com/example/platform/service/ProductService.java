package com.example.platform.service;

import com.example.platform.dto.ProductRequest;
import com.example.platform.dto.ProductResponse;
import com.example.platform.model.Product;
import com.example.platform.repository.ProductRepository;
import org.springframework.cache.annotation.CacheEvict;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
@Transactional(readOnly = true)
public class ProductService {

    private final ProductRepository repository;

    public ProductService(ProductRepository repository) {
        this.repository = repository;
    }

    @Cacheable(value = "products", key = "'all'")
    public List<ProductResponse> findAll() {
        return repository.findAll().stream().map(ProductResponse::from).toList();
    }

    public ProductResponse findById(Long id) {
        return repository.findById(id)
                .map(ProductResponse::from)
                .orElseThrow(() -> new RuntimeException("Product not found: " + id));
    }

    @Cacheable(value = "products", key = "#category")
    public List<ProductResponse> findByCategory(String category) {
        return repository.findByCategory(category).stream().map(ProductResponse::from).toList();
    }

    @Transactional
    @CacheEvict(value = "products", allEntries = true)
    public ProductResponse create(ProductRequest request) {
        Product product = new Product();
        product.setName(request.name());
        product.setDescription(request.description());
        product.setPrice(request.price());
        product.setCategory(request.category());
        return ProductResponse.from(repository.save(product));
    }
}
