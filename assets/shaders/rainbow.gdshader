shader_type canvas_item;

uniform float start_hue : hint_range(0.0, 1.0) = 0.0;
uniform float cycle_speed : hint_range(0.0, 2.0) = 0.1;
uniform float glow_strength : hint_range(1.0, 4.0) = 1.5;

vec3 hue2rgb(float h) {
    vec3 c = abs(mod(h * 6.0 + vec3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0;
    return clamp(c, 0.0, 1.0);
}

void fragment() {
	float dist = distance(UV, vec2(0.5));
	float core = 1.0 - smoothstep(0.0, 0.75, dist);

	float hue = fract(TIME * cycle_speed + start_hue);
	float pulse = glow_strength + 0.5 * sin(TIME * 3.0);
    vec3 edgeColor = hue2rgb(hue) * pulse;
    vec3 finalColor = mix(edgeColor, vec3(1.0), core);

    COLOR = vec4(finalColor, texture(TEXTURE, UV).a);
}
