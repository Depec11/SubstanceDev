#version 300 es

precision highp float;

uniform sampler2D u_texture;
uniform vec3 u_modulate;

in vec2 frag_texCoords;

out vec4 out_color;

void main() {
    vec4 texColor = texture(u_texture, frag_texCoords);
    out_color = vec4(texColor.rgb * u_modulate, texColor.a);
}