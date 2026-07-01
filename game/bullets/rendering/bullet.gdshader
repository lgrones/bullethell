shader_type canvas_item;

// NOTE: per-bullet atlas region is unsolved — 2D MultiMesh INSTANCE_CUSTOM
// aliases the instance color, so per-instance UV regions don't work yet.
// Hardcoded to the white star cell for now so the preview renders clean.
// Next: per-style MultiMesh + AtlasTexture instead (see memory notes).
uniform sampler2D atlas : source_color, filter_linear;

void fragment() {
	vec2 atlas_size = vec2(1408.0, 1152.0);
	vec2 uv = vec2(609.0, 33.0) / atlas_size + UV * (vec2(30.0, 30.0) / atlas_size);
	COLOR = texture(atlas, uv);
}
