shader_type canvas_item;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	// UV is 0..1 across the quad; center is (0.5, 0.5)
    float dist = distance(UV, vec2(0.5));
    // glow falls off from center: bright at 0, dark at edge
    float glow = smoothstep(0.5, 0.0, dist);
    vec3 core = vec3(1.0);              // white core
    vec3 halo = vec3(1.0, 0.4, 0.5);    // blue halo
    vec3 color = mix(halo, core, glow * glow);
    COLOR = vec4(color, glow);          // alpha = glow → transparent edges
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
