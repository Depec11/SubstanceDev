#version 330 core

uniform mat3x2 u_mvp;

layout (location = 0) in vec2 a_position;
layout (location = 1) in vec2 a_textureCoord;

out vec2 frag_texCoords;

void main() {
    vec2 clipPos = (u_mvp * vec3(a_position, 1.0)).xy;
    gl_Position = vec4(clipPos, 0.0, 1.0);

    frag_texCoords = a_textureCoord;
}